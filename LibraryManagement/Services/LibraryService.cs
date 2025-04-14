using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinRT.Interop;
using System.IO;

namespace LibraryManagement.Services
{

    public class LibraryService
    {
        private readonly LibraryDbContext _context = new LibraryDbContext();

        // Books
        public void AddBook(Book book)
        {
            _context.Books.Add(book);
            _context.SaveChanges();
        }

        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
        }

        public List<Book> GetAllAvailableBooks()
        {
            return _context.Books.Where(b => b.BorrowedBy == null).ToList();
        }

        public List<Book> GetBorrowedBooks()
        {
            return _context.Books.Where(b => b.BorrowedBy != null).ToList();
        }

        //Done
        public void LendBook(string ISBN, string customerId, int days)
        {
            var book = _context.Books.Find(ISBN);
            var customer = _context.Customers.Find(customerId);

            if (book == null || customer == null || book.IsBorrowed)
            {
                ShowMessageDialog("Book is not available.", "⛔ Action Denied ");
            }

            book.BorrowedBy = customerId;
            book.DueDate = DateTime.Now.AddSeconds(days); // Days lending period
            _context.SaveChanges();

        }
        
        public async Task ReturnBookAsync(string ISBN)
        {
            try
            {
                var book = _context.Books.Include(b => b.Reservations).FirstOrDefault(b => b.ISBN == ISBN);

                if (book != null && book.IsBorrowed)
                {

                    if (book.Reservations.Any()) // Assign to next in queue
                    {
                        var nextCustomer = book.Reservations.First();
                        var customerName = nextCustomer.Customer?.Name ?? "Unknown Customer";

                        // Show dialog to get the number of days
                        int? days = await ShowDaysInputDialog(
                            $"This book is reserved by ({customerName}).\r\n" +
                            "Please enter the number of days for their loan.\r\n" +
                            "Or remove the reservation first, then return the book.\r\n\r\n" +
                            $"هذا الكتاب محجوز بواسطة ({customerName}).\r\n" +
                            "من فضلك أدخل عدد الأيام لإعارته له.\r\n" +
                            "أو احذف الحجز أولًا ثم أعد الكتاب.");

                        if (days.HasValue)
                        {
                            book.BorrowedBy = nextCustomer.CustomerId;
                            book.DueDate = DateTime.Now.AddDays(days.Value);

                            // Remove from reservations only if a valid loan period was entered
                            book.Reservations.Remove(nextCustomer);
                            _context.SaveChanges();
                            return;
                        }
                        else
                        {
                            // Cancel was pressed, so don't remove the reservation
                            return;
                        }
                    }
                    book.BorrowedBy = null;
                    book.DueDate = null;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                var errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = $"An error occurred: {ex.Message}",
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await errorDialog.ShowAsync();
            }
        }
        
        public async Task<int?> ShowDaysInputDialog(string message)
        {
            var inputTextBox = new TextBox
            {
                AcceptsReturn = false,
                Height = 32,
                Text = "0" // Default value is 0
            };

            var dialog = new ContentDialog
            {
                Title = "⚠️ Warning",
                Content = new StackPanel
                {
                    Children =
            {
                new TextBlock { Text = message },
                inputTextBox
            }
                },
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel Return",
                XamlRoot = App.MainWindow.Content.XamlRoot
            };

            while (true) // Keep showing dialog until valid input is entered
            {
                var result = await dialog.ShowAsync();

                if (result == ContentDialogResult.Primary)
                {
                    string input = string.IsNullOrWhiteSpace(inputTextBox.Text) ? "0" : inputTextBox.Text;

                    if (int.TryParse(input, out int days) && days > 0)
                    {
                        return days; // Valid input, return value and exit loop
                    }
                    else
                    {
                        // Show an error and let the user try again
                        var errorDialog = new ContentDialog
                        {
                            Title = "Invalid Input",
                            Content = "Please enter a valid number greater than 0.",
                            CloseButtonText = "OK",
                            XamlRoot = App.MainWindow.Content.XamlRoot
                        };
                        await errorDialog.ShowAsync();
                        // Dialog will reopen in the next loop iteration
                    }
                }
                else
                {
                    return null; // User clicked Cancel, exit with null
                }
            }
        }
        public List<Customer> GetAllCustomers()
        {
            return _context.Customers.Include(c=>c.BorrowedBooks).ThenInclude(c => c.Reservations).ToList();
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public bool RemoveCustomer(string customerId)
        {
            var customer = _context.Customers
                           .Include(c => c.BorrowedBooks).ThenInclude(c=>c.Reservations)
                           .FirstOrDefault(c => c.CustomerID == customerId);

            if (customer == null) {
                 ShowMessageDialog("Customer has borrowed books and cannot be deleted.", "⛔ Action Denied");
                return false; // Customer not found
            }


            if (customer.BorrowedBooks.Count > 0)
            {

                 ShowMessageDialog("Customer has borrowed books and cannot be deleted.", "⛔ Action Denied");
                 return false ;
            }
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return true;
        }

        public bool RemoveBook(string isbn)
        {
            var book = _context.Books.Include(b=>b.Customer).FirstOrDefault(b => b.ISBN == isbn);
            if (book == null)
            {
                 ShowMessageDialog("Book Is NotFound.", "⛔ Action Denied");
                 return false; // book not found

            }



            if (book.IsBorrowed) 
            {
                 ShowMessageDialog("Cannot delete a borrowed book.", "⛔ Action Denied"); 
                 return false; 
                
            }
         
            _context.Books.Remove(book);
            _context.SaveChanges();
            return true;
           
        }


        // Method to show a ContentDialog
        public void ShowMessageDialog(string message, string title)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = App.MainWindow.Content.XamlRoot // Required in WinUI 3
            };

            dialog.ShowAsync();
        }

        public async Task<bool> ShowConfirmationDialog(string message)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Confirmation",
                Content = message,
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                XamlRoot = App.MainWindow.Content.XamlRoot // Required in WinUI 3
            };

            var result = await dialog.ShowAsync();
            return result == ContentDialogResult.Primary; // Returns true if "Yes" is clicked
        }

        public void AddReservation(Reservation reservation)

        {

            _context.Reservations.Add(reservation);

            _context.SaveChanges();

        }

        public void RemoveReservation(int reservationId)

        {

            var reservation = _context.Reservations.Find(reservationId);

            if (reservation != null)

            {

                _context.Reservations.Remove(reservation);

                _context.SaveChanges();

            }

        }

        public List<Reservation> GetAllReservations()

        {

            return _context.Reservations.Include(r => r.Book).Include(r => r.Customer).ToList();

        }


public async Task ExportReservationsToExcel(List<Reservation> reservations, Microsoft.UI.Xaml.Window window)
    {
        var picker = new FileSavePicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeChoices.Add("Excel Workbook", new List<string>() { ".xlsx" });
        picker.SuggestedFileName = "ReservationsReport";

        // Initialize with the window handle (required in WinUI 3)
        var hwnd = WindowNative.GetWindowHandle(window);
        InitializeWithWindow.Initialize(picker, hwnd);

        StorageFile file = await picker.PickSaveFileAsync();

        if (file == null)
            return; // User cancelled

        using (var stream = await file.OpenStreamForWriteAsync())
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Reservations");
                worksheet.Cell(1, 1).Value = "Book Title";
                worksheet.Cell(1, 2).Value = "Customer Name";
                worksheet.Cell(1, 3).Value = "Reservation Date";
                worksheet.Cell(1, 5).Value = "Total Reservations";
                worksheet.Cell(2, 5).Value = reservations.Count;
                worksheet.Cell(1, 6).Value = "Most Reserved Book";
                worksheet.Cell(2, 6).Value = GetTopBookName(reservations);
                worksheet.Cell(1, 7).Value = "Top Reserver";
                worksheet.Cell(2, 7).Value = GetTopCustomerName(reservations);

                for (int i = 0; i < reservations.Count; i++)
                {
                    var res = reservations[i];
                    worksheet.Cell(i + 2, 1).Value = res.Book.Title;
                    worksheet.Cell(i + 2, 2).Value = res.Customer.Name;
                    worksheet.Cell(i + 2, 3).Value = res.ReservationDate.ToString("dd/MM/yyyy");
                }

                workbook.SaveAs(stream);
            }
        }

        ShowMessageDialog($"Excel report saved successfully:\n{file.Path}", "✅ Done");
    }


    public string GetTopCustomerName(IEnumerable<Reservation> reservations)
        {
            return reservations
                .GroupBy(r => r.Customer.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";
        }

        public string GetTopAuthorName(IEnumerable<Book> books)
        {
            return books
                .GroupBy(r => r.Author)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";
        }

        public string GetTopBookName(IEnumerable<Reservation> reservations)
        {
            return reservations
                .GroupBy(r => r.Book.Title)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";
        }


    }

}
