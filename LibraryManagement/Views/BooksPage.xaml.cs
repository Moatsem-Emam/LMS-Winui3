using LibraryManagement.DTOS;
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
    public sealed partial class BooksPage : Page
    {
     
        public BooksPage()
        {
            this.InitializeComponent();
        }

        private readonly LibraryService _service = new();
        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Book book)
            {
                bool confirmDelete = await _service.ShowConfirmationDialog($"Are you sure you want to delete \"{book.Title}\"?");
                if (confirmDelete) { 
                    var viewModel = DataContext as BookViewModel;
                    viewModel?.RemoveBookCommand.Execute(book);
                }
            }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DataContext is BookViewModel viewModel)
            {
                viewModel.SearchText = SearchBox.Text;
                viewModel.FilterBooks();
            }
        }

        private void SearchOptionChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is BookViewModel viewModel)
            {
                viewModel.SearchByTitle = SearchByTitle.IsChecked ?? true;
                viewModel.FilterBooks();
            }
        }




    }
}
