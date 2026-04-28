using System.Windows;
using kursach.Models;
using kursach.Services;

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

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;
            string email = EmailTextBox.Text;
            string fullName = FullNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || 
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(fullName))
            {
                ErrorMessage.Text = "Заполните все поля!";
                return;
            }

            User newUser = new User
            {
                Username = username,
                Password = password,
                Email = email,
                FullName = fullName,
                IsAdmin = false
            };

            if (_dbService.CreateUser(newUser))
            {
                MessageBox.Show("Регистрация успешна! Теперь вы можете войти в систему.", "Успех");
                this.Close();
            }
            else
            {
                ErrorMessage.Text = "Ошибка регистрации. Возможно, пользователь уже существует.";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
