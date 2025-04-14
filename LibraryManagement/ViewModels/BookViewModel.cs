using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Services;
using LibraryManagement.Models;
using LibraryManagement.DTOS;
using System;
using Microsoft.VisualBasic;

namespace LibraryManagement.ViewModels
{
    public partial class BookViewModel : ObservableObject
    {
        public IRelayCommand AddBookCommand { get; }
        public IRelayCommand RemoveBookCommand { get; }

        private readonly LibraryService _service = new();

        [ObservableProperty]
        private ObservableCollection<Book> books;

        [ObservableProperty]
        private ObservableCollection<Book> filteredBooks; // For search results

        [ObservableProperty]
        private BookInput newBookInput = new(); // Holds user input

        [ObservableProperty]
        private string searchText = ""; // Holds the search text

        [ObservableProperty]
        private bool searchByTitle = true; // Default: Search by title

        [ObservableProperty]
        private int totalBooks;

        [ObservableProperty]
        private string topAuthor;

        [ObservableProperty]
        private int totalBorrowed;
        public BookViewModel()
        {
            books = new ObservableCollection<Book>(_service.GetAllBooks());
            filteredBooks = new ObservableCollection<Book>(books); // Initially same as Books

            AddBookCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrWhiteSpace(NewBookInput.Title) &&
                    !string.IsNullOrWhiteSpace(NewBookInput.Author))
                {
                    var book = new Book
                    {
                        Title = NewBookInput.Title,
                        Author = NewBookInput.Author
                    };

                    _service.AddBook(book);
                    books.Add(book);
                    FilteredBooks.Add(book); // Add to search results as well

                    // Clear the input fields
                    NewBookInput = new BookInput();
                    CalculateStatistics();
                }
            });

            RemoveBookCommand = new RelayCommand<Book>(book =>
            {
                if (book != null && !string.IsNullOrWhiteSpace(book.ISBN) && _service.RemoveBook(book.ISBN))
                {
                    Books.Remove(book);
                    FilteredBooks.Remove(book);
                }
                CalculateStatistics();
            });
            CalculateStatistics();
        }
       
        // ?? Filter Books based on search input
        public void FilterBooks()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredBooks = new ObservableCollection<Book>(books); // Reset if empty search
            }
            else
            {
                if (SearchByTitle)
                {
                    FilteredBooks = new ObservableCollection<Book>(
                        books.Where(b => b.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    );
                }
                else
                {
                    FilteredBooks = new ObservableCollection<Book>(
                        books.Where(b => b.Author.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    );
                }
            }
        }

        private void CalculateStatistics()
        {
            TotalBooks = books.Count;

            TotalBorrowed = books.Count(b=>b.IsBorrowed == true);
            // 👤 أكثر كاتب ناشر كتب
            TopAuthor = books
                .GroupBy(r => r.Author)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault() ?? "N/A";


            //TotalLoans = reservations.Count;

            //// 👤 أكثر عميل استعار كتب
            //TopCustomer = reservations
            //    .GroupBy(r => r.Customer.Name)
            //    .OrderByDescending(g => g.Count())
            //    .Select(g => g.Key)
            //    .FirstOrDefault() ?? "N/A";
        }
    }
}
