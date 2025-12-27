using BakeryApp.BLL.Services;
using System;
using System.Configuration;
using System.Windows;

namespace BakeryApp.UI
{
    public partial class LoginWindow : Window
    {
        private readonly UserService _userService;

        public LoginWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["BakeryDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Строка подключения 'BakeryDbConnection' не найдена в App.config.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }
            _userService = new UserService(connectionString);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_userService.AuthenticateUser(txtUsername.Text, txtPassword.Password))
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверное имя пользователя или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                App.ShowError("Произошла ошибка при входе.", ex);
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var registrationWindow = new RegistrationWindow(_userService);
            registrationWindow.Owner = this;
            registrationWindow.ShowDialog();
        }
    }
}
