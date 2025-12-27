using BakeryApp.DAL;
using BakeryApp.DAL.Models;
using System;
using System.Collections.Generic;

namespace BakeryApp.BLL.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;

        public ProductService(string connectionString)
        {
            _repository = new ProductRepository(connectionString);
        }

        public List<Product> GetAllProducts()
        {
            try
            {
                return _repository.GetAllProducts();
            }
            catch (Exception ex)
            {
                // In a real application, you would log this exception
                throw new Exception("An error occurred while fetching the products.", ex);
            }
        }

        public List<Ingredient> GetAllIngredients()
        {
            try
            {
                return _repository.GetAllIngredients();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the ingredients.", ex);
            }
        }

        public void SaveProduct(Product product)
        {
            // --- Business Logic: Validation ---
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Product cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(product.Name))
            {
                throw new ArgumentException("The product name cannot be empty.", nameof(product.Name));
            }

            if (product.Price < 0)
            {
                throw new ArgumentException("The price cannot be negative.", nameof(product.Price));
            }

            if (product.Weight <= 0)
            {
                throw new ArgumentException("The weight must be a positive number.", nameof(product.Weight));
            }

            try
            {
                if (product.Id == 0)
                {
                    _repository.AddProduct(product);
                }
                else
                {
                    _repository.UpdateProduct(product);
                }
            }
            catch (Exception ex)
            {
                // In a real application, you would log this exception
                throw new Exception($"An error occurred while saving the product '{product.Name}'.", ex);
            }
        }

        public void DeleteProduct(int productId)
        {
            if (productId <= 0)
            {
                throw new ArgumentException("Invalid product ID.", nameof(productId));
            }

            try
            {
                _repository.DeleteProduct(productId);
            }
            catch (Exception ex)
            {
                // In a real application, you would log this exception
                throw new Exception($"An error occurred while deleting the product with ID {productId}.", ex);
            }
        }
    }
}
