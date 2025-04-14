using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Models;
using LibraryManagement.Services;
using Microsoft.UI.Xaml;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryManagement.ViewModels
{
    public partial class LendingViewModel : ObservableObject
    {

        private readonly LibraryService _Service = new();

        [ObservableProperty]
        private ObservableCollection<Book> availableBooks;

        [ObservableProperty]
        private ObservableCollection<Book> borrowedBooks;
        public ObservableCollection<(Book Book, decimal LateFee)> BorrowedBooksWithFees;


        [ObservableProperty]
        private ObservableCollection<Customer> customers;

        [ObservableProperty]
        private Book selectedBook;

        [ObservableProperty]
        private Customer selectedCustomer;

        [ObservableProperty]
        private int days;

        [ObservableProperty]
        private int feesPerDay;

        [ObservableProperty]
        private int totalLoans;

        [ObservableProperty]
        private string topCustomer;
        public IRelayCommand LendBookCommand { get; }
        public IRelayCommand<Book> ReturnBookCommand { get; }
        public IRelayCommand<Book> CalcFeesCommand { get; }
        public IRelayCommand<Book> ReserveBookCommand { get; }
        

        public LendingViewModel()
        {
            availableBooks = new ObservableCollection<Book>(_Service.GetAllAvailableBooks());
            borrowedBooks = new ObservableCollection<Book>(_Service.GetBorrowedBooks());
            customers = new ObservableCollection<Customer>(_Service.GetAllCustomers());
            

            LendBookCommand = new RelayCommand(() =>
            {
                if (SelectedBook != null && SelectedCustomer != null)
                {

                    _Service.LendBook(SelectedBook.ISBN, SelectedCustomer.CustomerID, Days);
                    // Refresh lists
                    AvailableBooks = new ObservableCollection<Book>(_Service.GetAllAvailableBooks());
                    BorrowedBooks = new ObservableCollection<Book>(_Service.GetBorrowedBooks());
                    CalculateStatistics();

                }
            });
            ReturnBookCommand = new RelayCommand<Book>(async (book) =>
            {
                if (book != null)
                {
                    // Pass the XamlRoot from the view
                    await _Service.ReturnBookAsync(book.ISBN);
                    // Refresh lists
                    AvailableBooks = new ObservableCollection<Book>(_Service.GetAllAvailableBooks());
                    BorrowedBooks = new ObservableCollection<Book>(_Service.GetBorrowedBooks());
                    CalculateStatistics();
                }
            });
            CalculateStatistics();
        }

        private void CalculateStatistics()
        {

            TotalLoans = borrowedBooks.Count;

            // 👤 أكثر عميل استعار كتب
            TopCustomer = borrowedBooks
                .GroupBy(r => r.Customer.Name)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";
        }




    }

   

}


