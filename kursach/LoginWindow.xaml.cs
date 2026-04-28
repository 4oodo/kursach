using System.Windows;
using kursach.Models;
using kursach.Services;

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
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage.Text = "Заполните все поля!";
                return;
            }

            User user = _dbService.AuthenticateUser(username, password);

            if (user != null)
            {
                if (user.IsAdmin)
                {
                    // Открываем окно администратора
                    MainWindow adminWindow = new MainWindow();
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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}
