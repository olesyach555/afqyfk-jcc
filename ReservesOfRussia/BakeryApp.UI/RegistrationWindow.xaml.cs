using BakeryApp.BLL.Services;
using System;
using System.Windows;

namespace BakeryApp.UI
{
    public partial class RegistrationWindow : Window
    {
        private readonly UserService _userService;

        public RegistrationWindow(UserService userService)
        {
            InitializeComponent();
            _userService = userService;
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_userService.RegisterUser(txtUsername.Text, txtPassword.Password))
                {
                    MessageBox.Show("Регистрация прошла успешно. Теперь вы можете войти.", "Регистрация успешна", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Пользователь с таким именем уже существует.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentException argEx)
            {
                MessageBox.Show(argEx.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                App.ShowError("Произошла ошибка при регистрации.", ex);
            }
        }
    }
}
