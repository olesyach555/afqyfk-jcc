using BakeryApp.BLL.Services;
using BakeryApp.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BakeryApp.UI
{
    public partial class AddEditProductWindow : Window
    {
        private readonly ProductService _productService;
        private readonly Product _currentProduct;
        private List<Ingredient> _allIngredients;

        public AddEditProductWindow(ProductService productService, Product productToEdit = null)
        {
            InitializeComponent();
            _productService = productService;

            if (productToEdit != null)
            {
                // Editing an existing product
                _currentProduct = productToEdit;
                Title = "Изменить продукт";
            }
            else
            {
                // Creating a new product
                _currentProduct = new Product();
                Title = "Добавить новый продукт";
            }

            LoadAllIngredients();
            DataContext = _currentProduct;
        }

        private void LoadAllIngredients()
        {
            try
            {
                _allIngredients = _productService.GetAllIngredients();
                cmbIngredients.ItemsSource = _allIngredients;
            }
            catch (Exception ex)
            {
                App.ShowError("Не удалось загрузить список ингредиентов.", ex);
                this.Close();
            }
        }

        private void btnAddIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (cmbIngredients.SelectedItem is Ingredient selectedIngredient &&
                double.TryParse(txtQuantity.Text, out double quantity))
            {
                // Check if the ingredient is already in the list
                if (!_currentProduct.Ingredients.Any(pi => pi.Ingredient.Id == selectedIngredient.Id))
                {
                    _currentProduct.Ingredients.Add(new ProductIngredient
                    {
                        IngredientId = selectedIngredient.Id,
                        Ingredient = selectedIngredient,
                        Quantity = quantity
                    });

                    // Refresh the DataGrid
                    ingredientsGrid.ItemsSource = null;
                    ingredientsGrid.ItemsSource = _currentProduct.Ingredients;
                }
                else
                {
                    MessageBox.Show("Этот ингредиент уже в списке.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите ингредиент и введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnRemoveIngredient_Click(object sender, RoutedEventArgs e)
        {
            if (ingredientsGrid.SelectedItem is ProductIngredient selectedProductIngredient)
            {
                _currentProduct.Ingredients.Remove(selectedProductIngredient);
                // Refresh the DataGrid
                ingredientsGrid.ItemsSource = null;
                ingredientsGrid.ItemsSource = _currentProduct.Ingredients;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // The UI is bound to _currentProduct, so it's already updated.
                // We just need to call the service.
                _productService.SaveProduct(_currentProduct);
                DialogResult = true; // Signals success to the main window
            }
            catch (ArgumentException argEx)
            {
                // Catches validation errors from BLL
                MessageBox.Show(argEx.Message, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                // Catches other errors (e.g., database)
                App.ShowError("Произошла ошибка при сохранении продукта.", ex);
            }
        }
    }
}
