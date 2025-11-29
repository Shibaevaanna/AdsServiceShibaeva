using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AdsServiceShibaeva.Pages
{
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void buttonauth_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(logintextbox.Text) || string.IsNullOrEmpty(passwordbox.Password))
            {
                MessageBox.Show("Введите логин или пароль");
                return;
            }

            // Хардкодные пользователи для тестирования
            if (CheckHardcodedUser(logintextbox.Text, passwordbox.Password))
            {
                MessageBox.Show("Успешный вход!");
                NavigationService.Navigate(new UserPage());
                return;
            }

            // Если хардкодные данные не подошли, проверяем базу данных
            using (var db = new AdsServiceShibaevaEntities())
            {
                var user = db.users.AsNoTracking().FirstOrDefault(u => u.user_login == logintextbox.Text && u.user_password == passwordbox.Password);
                if (user == null)
                {
                    MessageBox.Show("Пользователь с такими данными не найден");
                }
                else
                {
                    MessageBox.Show("Успешный вход!");
                    NavigationService.Navigate(new UserPage());
                }
            }
        }

        private bool CheckHardcodedUser(string login, string password)
        {
            // Хардкодные пользователи
            var hardcodedUsers = new[]
            {
                new { Login = "admin", Password = "admin" },
                new { Login = "user", Password = "123" },
                new { Login = "test", Password = "test" },
                new { Login = "user1", Password = "password" }
            };

            return hardcodedUsers.Any(u => u.Login == login && u.Password == password);
        }
    }
}