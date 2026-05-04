using System;
using System.Windows;
using kursach.Services;
using kursach.Utilities;

namespace kursach
{
    public partial class RegisterWindow : Window
    {
        private DatabaseService _dbService;

        public RegisterWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void IsAdminCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AdminKeyLabel.Visibility = Visibility.Visible;
            AdminKeyTextBox.Visibility = Visibility.Visible;
        }

        private void IsAdminCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AdminKeyLabel.Visibility = Visibility.Collapsed;
            AdminKeyTextBox.Visibility = Visibility.Collapsed;
            AdminKeyTextBox.Clear();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;
            bool isAdmin = IsAdminCheckBox.IsChecked ?? false;
            string adminKey = AdminKeyTextBox.Text.Trim();

            // Валидация
            if (string.IsNullOrWhiteSpace(username))
            {
                ErrorMessage.Text = "Введите имя пользователя!";
                return;
            }

            if (username.Length < 3)
            {
                ErrorMessage.Text = "Имя пользователя должно быть не менее 3 символов!";
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage.Text = "Введите пароль!";
                return;
            }

            if (password.Length < 6)
            {
                ErrorMessage.Text = "Пароль должен быть не менее 6 символов!";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorMessage.Text = "Пароли не совпадают!";
                return;
            }

            try
            {
                // Проверка наличия пользователя
                if (_dbService.UserExists(username))
                {
                    ErrorMessage.Text = "Пользователь с таким именем уже существует!";
                    return;
                }

                // Проверка ключа администратора
                int roleID = _dbService.GetRoleIDByName("User"); // По умолчанию роль User
                if (isAdmin)
                {
                    if (string.IsNullOrWhiteSpace(adminKey))
                    {
                        ErrorMessage.Text = "Введите ключ администратора!";
                        return;
                    }

                    if (!_dbService.ValidateAdminKey(adminKey))
                    {
                        ErrorMessage.Text = "Неверный ключ администратора!";
                        return;
                    }

                    roleID = _dbService.GetRoleIDByName("Admin"); // Роль Admin
                }

                // Хеширование пароля
                string passwordHash = PasswordHasher.HashPassword(password);

                // Создание пользователя
                if (_dbService.CreateUser(username, passwordHash, roleID))
                {
                    MessageBox.Show("✅ Регистрация успешна! Теперь вы можете войти в систему.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    ErrorMessage.Text = "❌ Ошибка при сохранении пользователя в БД. Проверьте подключение.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"❌ Ошибка: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Registration error: {ex}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
