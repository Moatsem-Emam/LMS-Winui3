using LibraryManagement.Views;
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

namespace LibraryManagement
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public Frame RootFrame => ContentFrame;
        public MainWindow()
        {
            this.InitializeComponent();
            ContentFrame.Navigate(typeof(BooksPage));
        }
        // Navigate to HomePage when the app starts
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            string PageName = args.InvokedItemContainer.Tag as string;
            switch (PageName)
            {
                case "Books Page":
                    ContentFrame.Navigate(typeof(BooksPage));
                    break;
                case "Customers Page":
                    ContentFrame.Navigate(typeof(CustomersPage));
                    break;
                case "Lending Page":
                    ContentFrame.Navigate(typeof(LendingPage));
                    break;
                case "Reservations Page":
                    ContentFrame.Navigate(typeof(ReservationsPage));
                    break;
                
                default:
                    break;
            }
        }

        //private void myButton_Click(object sender, RoutedEventArgs e)
        //{
        //    myButton.Content = "Clicked";
        //}
    }


}
