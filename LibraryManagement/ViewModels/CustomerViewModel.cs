using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.ViewModels
{
    public partial class CustomerViewModel : ObservableObject
    {
        public IRelayCommand AddCustomerCommand { get; }
        public IRelayCommand RemoveCustomerCommand { get; }
        public RelayCommand<Customer> ViewDetailsCommand { get; }

        private readonly LibraryService _service = new();

        [ObservableProperty]
        private ObservableCollection<Customer> customers;

        [ObservableProperty]
        private string name; // Holds user input

        public CustomerViewModel()
        {
            customers = new ObservableCollection<Customer>(_service.GetAllCustomers());
            AddCustomerCommand = new RelayCommand(() =>
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    
                    var customer = new Customer { Name = Name };
                    _service.AddCustomer(customer);
                    customers.Add(customer);

                    // Clear the input fields
                    Name = string.Empty;
                }
            });
            RemoveCustomerCommand = new RelayCommand<Customer>(customer =>
            {
                if (customer != null && _service.RemoveCustomer(customer.CustomerID))
                {
                    Customers.Remove(customer);
                }
            });
            ViewDetailsCommand = new RelayCommand<Customer>(ViewDetails);

        }


        

        private void ViewDetails(Customer selectedCustomer)
        {
            if (selectedCustomer != null)
            {
                var nav = App.MainRootFrame;
                nav.Navigate(typeof(CustomerDetailsPage), selectedCustomer);
            }
        }

    }
}
