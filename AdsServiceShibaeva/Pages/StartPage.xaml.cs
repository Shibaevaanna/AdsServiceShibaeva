using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;

namespace AdsServiceShibaeva.Pages
{
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
            var currentAds = AdsServiceShibaevaEntities.GetContext().ads.ToList();
            listview_ads.ItemsSource = currentAds;
        }

        private void clearFilter_Click(object sender, RoutedEventArgs e)
        {
            titlefilter.Text = "";
            sortcombobox.SelectedIndex = 0;
        }

        private void titlefilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateAds();
        }

        private void sortcombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAds();
        }

        private void authButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void UpdateAds()
        {
            if (!IsInitialized)
            {
                return;
            }

            try
            {
                List<ads> currentAds = AdsServiceShibaevaEntities.GetContext().ads
                    .Include(a => a.cities)
                    .Include(a => a.categories)
                    .Include(a => a.ad_types)
                    .Include(a => a.ad_statuses)
                    .ToList();

                
                if (!string.IsNullOrWhiteSpace(titlefilter.Text))
                {
                    currentAds = currentAds.Where(x => x.ad_title.ToLower().Contains(titlefilter.Text.ToLower())).ToList();
                }

                
                switch (sortcombobox.SelectedIndex)
                {
                    case 0: 
                        currentAds = currentAds.OrderBy(x => x.cities.city_name).ToList();
                        break;
                    case 1: 
                        currentAds = currentAds.OrderBy(x => x.categories.category_name).ToList();
                        break;
                    case 2: 
                        currentAds = currentAds.OrderBy(x => x.ad_types.type_name).ToList();
                        break;
                    case 3: 
                        currentAds = currentAds.OrderBy(x => x.ad_statuses.status_name).ToList();
                        break;
                    default:
                        currentAds = currentAds.OrderBy(x => x.cities.city_name).ToList();
                        break;
                }

                listview_ads.ItemsSource = currentAds;
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}