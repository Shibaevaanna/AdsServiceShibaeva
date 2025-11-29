using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Data.Entity;

namespace AdsServiceShibaeva.Pages
{
    public partial class AddEditAdPage : Page, INotifyPropertyChanged
    {
        private ads _currentAd;
        private bool _isEditMode;

        public ads CurrentAd
        {
            get => _currentAd;
            set
            {
                _currentAd = value;
                OnPropertyChanged(nameof(CurrentAd));
            }
        }

        public string PageTitle => _isEditMode ? "Редактирование объявления" : "Добавление объявления";
        public Visibility DeleteButtonVisibility => _isEditMode ? Visibility.Visible : Visibility.Collapsed;

        public AddEditAdPage(ads ad = null)
        {
            InitializeComponent();
            DataContext = this;

            _isEditMode = ad != null;
            CurrentAd = ad ?? new ads();

            Loaded += AddEditAdPage_Loaded;
        }

        private void AddEditAdPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxData();
        }

        private void LoadComboBoxData()
        {
            try
            {
                var context = AdsServiceShibaevaEntities.GetContext();

                // Загружаем города
                var cities = context.cities.ToList();
                CityComboBox.ItemsSource = cities;
                Console.WriteLine($"Загружено городов: {cities.Count}");

                // Загружаем категории
                var categories = context.categories.ToList();
                CategoryComboBox.ItemsSource = categories;
                Console.WriteLine($"Загружено категорий: {categories.Count}");

                // Загружаем типы объявлений
                var types = context.ad_types.ToList();
                TypeComboBox.ItemsSource = types;
                Console.WriteLine($"Загружено типов: {types.Count}");

                // Загружаем статусы
                var statuses = context.ad_statuses.ToList();
                StatusComboBox.ItemsSource = statuses;
                Console.WriteLine($"Загружено статусов: {statuses.Count}");

                // Устанавливаем значения по умолчанию для нового объявления
                if (!_isEditMode)
                {
                    CurrentAd.post_date = DateTime.Now;
                    CurrentAd.created_date = DateTime.Now;
                    CurrentAd.price = 0;

                    // Устанавливаем первый статус (обычно "Активно")
                    if (statuses.Count > 0)
                    {
                        CurrentAd.status_id = statuses[0].status_id;
                        StatusComboBox.SelectedValue = statuses[0].status_id;
                    }

                    // Устанавливаем первый город
                    if (cities.Count > 0)
                    {
                        CurrentAd.city_id = cities[0].city_id;
                        CityComboBox.SelectedValue = cities[0].city_id;
                    }

                    // Устанавливаем первую категорию
                    if (categories.Count > 0)
                    {
                        CurrentAd.category_id = categories[0].category_id;
                        CategoryComboBox.SelectedValue = categories[0].category_id;
                    }

                    // Устанавливаем первый тип
                    if (types.Count > 0)
                    {
                        CurrentAd.type_id = types[0].type_id;
                        TypeComboBox.SelectedValue = types[0].type_id;
                    }
                }
                else
                {
                    // Для редактирования устанавливаем текущие значения
                    CityComboBox.SelectedValue = CurrentAd.city_id;
                    CategoryComboBox.SelectedValue = CurrentAd.category_id;
                    TypeComboBox.SelectedValue = CurrentAd.type_id;
                    StatusComboBox.SelectedValue = CurrentAd.status_id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData())
                return;

            try
            {
                var context = AdsServiceShibaevaEntities.GetContext();

                if (!_isEditMode)
                {
                    // Добавление нового объявления
                    CurrentAd.created_date = DateTime.Now;
                    CurrentAd.updated_date = DateTime.Now;

                    // Хардкод user_id (в реальном приложении брать из текущей сессии)
                    CurrentAd.user_id = 1;

                    context.ads.Add(CurrentAd);
                }
                else
                {
                    // Редактирование существующего
                    var existingAd = context.ads.Find(CurrentAd.ad_id);
                    if (existingAd != null)
                    {
                        existingAd.ad_title = CurrentAd.ad_title;
                        existingAd.ad_description = CurrentAd.ad_description;
                        existingAd.price = CurrentAd.price;
                        existingAd.city_id = CurrentAd.city_id;
                        existingAd.category_id = CurrentAd.category_id;
                        existingAd.type_id = CurrentAd.type_id;
                        existingAd.status_id = CurrentAd.status_id;
                        existingAd.post_date = CurrentAd.post_date;
                        existingAd.updated_date = DateTime.Now;
                    }
                }

                context.SaveChanges();
                MessageBox.Show(_isEditMode ? "Объявление успешно обновлено!" : "Объявление успешно добавлено!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private bool ValidateData()
        {
            if (string.IsNullOrWhiteSpace(CurrentAd.ad_title))
            {
                MessageBox.Show("Введите заголовок объявления");
                TitleTextBox.Focus();
                return false;
            }

            if (CurrentAd.price <= 0)
            {
                MessageBox.Show("Введите корректную цену");
                PriceTextBox.Focus();
                return false;
            }

            if (CurrentAd.city_id == 0)
            {
                MessageBox.Show("Выберите город");
                CityComboBox.Focus();
                return false;
            }

            if (CurrentAd.category_id == 0)
            {
                MessageBox.Show("Выберите категорию");
                CategoryComboBox.Focus();
                return false;
            }

            if (CurrentAd.type_id == 0)
            {
                MessageBox.Show("Выберите тип объявления");
                TypeComboBox.Focus();
                return false;
            }

            if (CurrentAd.status_id == 0)
            {
                MessageBox.Show("Выберите статус");
                StatusComboBox.Focus();
                return false;
            }

            return true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить это объявление?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var context = AdsServiceShibaevaEntities.GetContext();
                    var adToDelete = context.ads.Find(CurrentAd.ad_id);
                    if (adToDelete != null)
                    {
                        context.ads.Remove(adToDelete);
                        context.SaveChanges();
                        MessageBox.Show("Объявление успешно удалено!");
                        NavigationService.GoBack();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}