using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace AdsServiceShibaeva.Pages
{
    public partial class UserAdsPage : Page
    {
        public UserAdsPage()
        {
            InitializeComponent();
            LoadUserAds();
        }

        private void LoadUserAds()
        {
            try
            {
                
                int currentUserId = 1;

                var userAds = AdsServiceShibaevaEntities.GetContext().ads
                    .Where(a => a.user_id == currentUserId)
                    .Include(a => a.cities)
                    .Include(a => a.categories)
                    .Include(a => a.ad_types)
                    .Include(a => a.ad_statuses)
                    .ToList();

                AdsListView.ItemsSource = userAds;

                
                NoAdsText.Visibility = userAds.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки объявлений: {ex.Message}");
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditAdPage());
        }

        private void CompletedButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserCompletedAdsPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedAd = AdsListView.SelectedItem as ads;
            if (selectedAd != null)
            {
                NavigationService.Navigate(new AddEditAdPage(selectedAd));
            }
            else
            {
                MessageBox.Show("Выберите объявление для редактирования");
            }
        }

        private void CompleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedAd = AdsListView.SelectedItem as ads;
            if (selectedAd != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите завершить это объявление?",
                    "Подтверждение завершения", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = AdsServiceShibaevaEntities.GetContext();

                        
                        var completedStatusId = context.ad_statuses
                            .Where(s => s.status_name == "Завершено")
                            .Select(s => s.status_id)
                            .FirstOrDefault();

                        if (completedStatusId == 0)
                        {
                            MessageBox.Show("Статус 'Завершено' не найден в базе данных");
                            return;
                        }

                        var adToUpdate = context.ads.Find(selectedAd.ad_id);
                        if (adToUpdate != null)
                        {
                            adToUpdate.status_id = completedStatusId;
                            adToUpdate.updated_date = DateTime.Now;
                            context.SaveChanges();

                            MessageBox.Show("Объявление завершено!");
                            LoadUserAds(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при завершении объявления: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите объявление для завершения");
            }
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedAd = AdsListView.SelectedItem as ads;
            if (selectedAd != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить это объявление?", "Подтверждение удаления",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = AdsServiceShibaevaEntities.GetContext();
                        var adToDelete = context.ads.Find(selectedAd.ad_id);
                        if (adToDelete != null)
                        {
                            context.ads.Remove(adToDelete);
                            context.SaveChanges();
                            MessageBox.Show("Объявление успешно удалено!");
                            LoadUserAds(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите объявление для удаления");
            }
        }

        
        private void AdsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EditMenuItem_Click(sender, e);
        }
    }
}