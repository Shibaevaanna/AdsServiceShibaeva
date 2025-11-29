using System.Windows;
using AdsServiceShibaeva.Pages;

namespace AdsServiceShibaeva
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new StartPage());
        }

        private void backbutton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }
    }
}