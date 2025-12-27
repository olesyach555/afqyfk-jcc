using BakeryApp.BLL.Services;
using BakeryApp.DAL;
using BakeryApp.DAL.Models;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace BakeryApp.UI
{
    public partial class MainWindow : Window
    {
        private readonly ProductService _productService;

        public MainWindow()
        {
            InitializeComponent();
            string connectionString = ConfigurationManager.ConnectionStrings["BakeryDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Строка подключения 'BakeryDbConnection' не найдена в App.config.", "Ошибка конфигурации", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
                return;
            }

            // --- Initialize the database on startup ---
            try
            {
                DatabaseSetup.InitializeDatabase(connectionString);
            }
            catch (Exception ex)
            {
                App.ShowError("Произошла критическая ошибка при настройке базы данных.", ex);
                Application.Current.Shutdown();
                return;
            }
            // ------------------------------------------

            _productService = new ProductService(connectionString);
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = _productService.GetAllProducts();
                productsListBox.ItemsSource = products;
                ClearDetails();
            }
            catch (Exception ex)
            {
                App.ShowError("Не удалось загрузить продукты.", ex);
            }
        }

        private void ClearDetails()
        {
            detailsPanel.DataContext = null;
        }

        private void productsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            detailsPanel.DataContext = productsListBox.SelectedItem as Product;
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addEditWindow = new AddEditProductWindow(_productService);
            addEditWindow.Owner = this;
            if (addEditWindow.ShowDialog() == true)
            {
                LoadProducts(); // Refresh the list
            }
        }

        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (productsListBox.SelectedItem is Product selectedProduct)
            {
                var addEditWindow = new AddEditProductWindow(_productService, selectedProduct);
                addEditWindow.Owner = this;
                if (addEditWindow.ShowDialog() == true)
                {
                    LoadProducts(); // Refresh the list
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для изменения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (productsListBox.SelectedItem is Product selectedProduct)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить '{selectedProduct.Name}'?", "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _productService.DeleteProduct(selectedProduct.Id);
                        LoadProducts(); // Refresh the list
                    }
                    catch (Exception ex)
                    {
                        App.ShowError($"Не удалось удалить продукт '{selectedProduct.Name}'.", ex);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите продукт для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
