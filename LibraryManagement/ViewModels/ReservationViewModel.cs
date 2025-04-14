using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Models;
using LibraryManagement.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using static System.Reflection.Metadata.BlobBuilder;

namespace LibraryManagement.ViewModels
{
    public partial class ReservationViewModel : ObservableObject
    {
        private readonly LibraryService _service = new();

        [ObservableProperty]
        private ObservableCollection<Book> availableBooks;

        [ObservableProperty]
        private ObservableCollection<Customer> customers;

        [ObservableProperty]
        private ObservableCollection<Reservation> filteredReservations; // For search results

        [ObservableProperty]
        private string searchText = ""; // Holds the search text

        [ObservableProperty]
        private bool searchByTitle = true; // Default: Search by title

        [ObservableProperty]
        private ObservableCollection<Reservation> reservations;

        [ObservableProperty]
        private Book selectedBook;

        [ObservableProperty]
        private Customer selectedCustomer;

        [ObservableProperty]
        private int totalReservations;

        [ObservableProperty]
        private string mostReservedBook;

        [ObservableProperty]
        private string topCustomer;

        [ObservableProperty]
        private bool isCustomerComboBoxEnabled = false; // Default to false (disabled)
        public IRelayCommand AddReservationCommand { get; }
        public IRelayCommand<Reservation> RemoveReservationCommand { get; }

        public ReservationViewModel()
        {
            availableBooks = new ObservableCollection<Book>(_service.GetBorrowedBooks());
            reservations = new ObservableCollection<Reservation>(_service.GetAllReservations());
            filteredReservations = new ObservableCollection<Reservation>(reservations); // Initially same as reservations

            // Initialize customers without filtering initially
            customers = new ObservableCollection<Customer>(_service.GetAllCustomers());

            AddReservationCommand = new RelayCommand(() =>
            {
                if (SelectedBook != null && SelectedCustomer != null)
                {
                    var reservation = new Reservation
                    {
                        BookISBN = SelectedBook.ISBN,
                        CustomerId = SelectedCustomer.CustomerID,
                        ReservationDate = DateTime.Now
                    };

                    _service.AddReservation(reservation);
                    reservations.Add(reservation);
                    FilteredReservations.Add(reservation);

                    AvailableBooks = new ObservableCollection<Book>(_service.GetBorrowedBooks());

                    CalculateStatistics(); // 🔄 تحديث الإحصائيات
                }
            });

            RemoveReservationCommand = new RelayCommand<Reservation>(reservation =>
            {
                if (reservation != null)
                {
                    _service.RemoveReservation(reservation.ReservationId);
                    reservations.Remove(reservation);
                    FilteredReservations.Remove(reservation);

                    AvailableBooks = new ObservableCollection<Book>(_service.GetBorrowedBooks());

                    CalculateStatistics(); // 🔄 تحديث الإحصائيات
                }
            });

            CalculateStatistics();
        }
        // ?? Filter Reservations based on search input
        public void FilterReservations()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredReservations = new ObservableCollection<Reservation>(reservations); // Reset if empty search
            }
            else
            {
                if (SearchByTitle)
                {
                    FilteredReservations = new ObservableCollection<Reservation>(
                        reservations.Where(r => r.Book.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    );
                }
                else
                {
                    FilteredReservations = new ObservableCollection<Reservation>(
                        reservations.Where(r =>r.Customer.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    );
                }
            }
        }
        private void CalculateStatistics()
        {
            TotalReservations = reservations.Count;

            // 📖 أكثر كتاب تم حجزه
            MostReservedBook = _service.GetTopBookName(reservations);

            // 👤 أكثر عميل حجز
            TopCustomer = _service.GetTopCustomerName(reservations);
        }

        partial void OnSelectedBookChanged(Book value)
        {
            if (value != null)
            {
                // Enable the customer ComboBox
                IsCustomerComboBoxEnabled = true;

                // Get the list of reservations for the selected book
                var currentReservations = Reservations
                    .Where(r => r.BookISBN == value.ISBN)
                    .ToList();

                // Get the customers who have reservations for the selected book
                var reservedCustomerIds = currentReservations.Select(r => r.CustomerId).ToList();

                // Filter customers to exclude the one currently borrowing the book
                var eligibleCustomers = _service.GetAllCustomers()
                    .Where(c => c.CustomerID != value.BorrowedBy)
                    .Where(c => !reservedCustomerIds.Contains(c.CustomerID))
                    .ToList();

                Customers = new ObservableCollection<Customer>(eligibleCustomers);
            }
            else
            {
                // Disable the customer ComboBox if no book is selected
                IsCustomerComboBoxEnabled = false;
            }
        }
    }
}
