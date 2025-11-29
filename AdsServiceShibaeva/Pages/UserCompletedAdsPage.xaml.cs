using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace AdsServiceShibaeva.Pages
{
    public partial class UserCompletedAdsPage : Page
    {
        public UserCompletedAdsPage()
        {
            InitializeComponent();
            LoadCompletedAds();
        }

        private void LoadCompletedAds()
        {
            try
            {
                
                int currentUserId = 1;

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

                
                var completedAds = context.ads
                    .Where(a => a.user_id == currentUserId && a.status_id == completedStatusId)
                    .Include(a => a.cities)
                    .Include(a => a.categories)
                    .Include(a => a.ad_types)
                    .Include(a => a.ad_statuses)
                    .OrderByDescending(a => a.updated_date) 
                    .ToList();

                CompletedAdsListView.ItemsSource = completedAds;

                if (completedAds.Count == 0)
                {
                    NoCompletedAdsPanel.Visibility = Visibility.Visible;
                    CompletedAdsListView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    NoCompletedAdsPanel.Visibility = Visibility.Collapsed;
                    CompletedAdsListView.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки завершенных объявлений: {ex.Message}");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ViewDetailsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedAd = CompletedAdsListView.SelectedItem as ads;
            if (selectedAd != null)
            {
                
                MessageBox.Show(
                    $"Заголовок: {selectedAd.ad_title}\n" +
                    $"Описание: {selectedAd.ad_description}\n" +
                    $"Цена: {selectedAd.price:C}\n" +
                    $"Город: {selectedAd.cities?.city_name}\n" +
                    $"Категория: {selectedAd.categories?.category_name}\n" +
                    $"Тип: {selectedAd.ad_types?.type_name}\n" +
                    $"Дата публикации: {selectedAd.post_date:dd.MM.yyyy}\n" +
                    $"Дата завершения: {selectedAd.updated_date:dd.MM.yyyy}",
                    "Детали объявления",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Выберите объявление для просмотра деталей");
            }
        }

        private void ReactivateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var selectedAd = CompletedAdsListView.SelectedItem as ads;
            if (selectedAd != null)
            {
                if (MessageBox.Show("Вы уверены, что хотите вернуть это объявление в активные?",
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var context = AdsServiceShibaevaEntities.GetContext();

                        
                        var activeStatusId = context.ad_statuses
                            .Where(s => s.status_name == "Активно")
                            .Select(s => s.status_id)
                            .FirstOrDefault();

                        if (activeStatusId == 0)
                        {
                            MessageBox.Show("Статус 'Активно' не найден в базе данных");
                            return;
                        }

                        var adToUpdate = context.ads.Find(selectedAd.ad_id);
                        if (adToUpdate != null)
                        {
                            adToUpdate.status_id = activeStatusId;
                            adToUpdate.updated_date = DateTime.Now;
                            context.SaveChanges();

                            MessageBox.Show("Объявление возвращено в активные!");
                            LoadCompletedAds(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при возврате объявления: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите объявление для возврата в активные");
            }
        }

        private void GoToActiveButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserAdsPage());
        }

        
        private void CompletedAdsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ViewDetailsMenuItem_Click(sender, e);
        }
    }
}