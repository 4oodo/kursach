using System;
using System.Windows;
using System.Windows.Controls;
using kursach.Models;
using kursach.Services;
using kursach.Utilities;

namespace kursach
{
    public partial class LoginWindow : Window
    {
        private DatabaseService _dbService;

        public LoginWindow()
        {
            InitializeComponent();
            _dbService = new DatabaseService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage.Text = "Заполните все поля!";
                return;
            }

            try
            {
                string passwordHash = PasswordHasher.HashPassword(password);
                User user = _dbService.AuthenticateUser(username, passwordHash);

                if (user != null)
                {
                    if (user.RoleName == "Admin")
                    {
                        // Открываем окно администратора
                        MainWindow adminWindow = new MainWindow(user);
                        adminWindow.Show();
                    }
                    else
                    {
                        // Открываем окно пользователя
                        UserWindow userWindow = new UserWindow(user);
                        userWindow.Show();
                    }

                    this.Close();
                }
                else
                {
                    ErrorMessage.Text = "Неверное имя пользователя или пароль!";
                    PasswordBox.Clear();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Ошибка подключения: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }

        private void LoginButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Button).Background = System.Windows.Media.Brushes.LimeGreen;
        }

        private void LoginButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Button).Background = System.Windows.Media.Brushes.Green;
        }

        private void RegisterButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Button).Background = System.Windows.Media.Brushes.DeepSkyBlue;
        }

        private void RegisterButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            (sender as Button).Background = System.Windows.Media.Brushes.RoyalBlue;
        }
    }
}
