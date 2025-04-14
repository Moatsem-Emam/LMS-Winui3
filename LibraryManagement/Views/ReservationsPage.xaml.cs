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
    public sealed partial class ReservationsPage : Page
    {
        LibraryService _service = new LibraryService();
        public ReservationsPage()
        {
            this.InitializeComponent();
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Reservation reservation)
            {
                bool confirmDelete = await _service.ShowConfirmationDialog($"Are you sure you want to delete \"{reservation.Customer.Name} reservation\"?");

                if (confirmDelete)
                {
                    var viewModel = DataContext as ReservationViewModel;
                    viewModel?.RemoveReservationCommand.Execute(reservation);
                }
            }

        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is ReservationViewModel viewModel)
            {
                viewModel.SearchText = SearchBox.Text;
                viewModel.FilterReservations();
            }
        }

        private void SearchOptionChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is ReservationViewModel viewModel)
            {
                viewModel.SearchByTitle = SearchByTitle.IsChecked ?? true;
                viewModel.FilterReservations();
            }
        }

        private async void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ReservationViewModel;
            if (viewModel == null || viewModel.FilteredReservations.Count == 0)
            {
                _service.ShowMessageDialog("No data to export.","⛔ Action Denied");
                return;
            }
            await _service.ExportReservationsToExcel(viewModel.FilteredReservations.ToList(), App.MainWindow);

            
        }

    }
}
