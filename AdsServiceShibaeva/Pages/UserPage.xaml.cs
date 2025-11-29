using System.Windows;
using System.Windows.Controls;

namespace AdsServiceShibaeva.Pages
{
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
        }

        private void MyAdsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserAdsPage());
        }

        private void AddAdButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditAdPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StartPage());
        }
    }
}