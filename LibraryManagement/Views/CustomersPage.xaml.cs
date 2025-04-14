using LibraryManagement.Models;
using LibraryManagement.Services;
using LibraryManagement.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LibraryManagement.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CustomersPage : Page
    {
        public CustomersPage()
        {
            this.InitializeComponent();
        }
        private readonly LibraryService _service = new();
        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Customer customer)
            {
                bool confirmDelete = await _service.ShowConfirmationDialog($"Are you sure you want to delete \"{customer.Name}\"?");
                if (confirmDelete)
                {
                    var viewModel = DataContext as CustomerViewModel;
                    viewModel?.RemoveCustomerCommand.Execute(customer);
                }
            }
        }

        private async void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Customer customer)
            {

                var viewModel = DataContext as CustomerViewModel;
                viewModel?.ViewDetailsCommand.Execute(customer);
                
            }
        }
    }
}
