using BakeryApp.BLL.Services;
using BakeryApp.DAL;
using BakeryApp.DAL.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace BakeryApp.Tests
{
    [TestClass]
    public class UserServiceTests
    {
        private static UserService _userService;
        private static string _connectionString;
        private static string _dbFile;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BakeryDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'BakeryDbConnection' not found in App.config.");
            }

            var builder = new SQLiteConnectionStringBuilder(_connectionString);
            _dbFile = builder.DataSource;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
            DatabaseSetup.InitializeDatabase(_connectionString);
            _userService = new UserService(_connectionString);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
        }

        [TestMethod]
        public void RegisterUser_HappyPath_ShouldSucceed()
        {
            bool result = _userService.RegisterUser("testuser", "password123");
            Assert.IsTrue(result);
            Assert.IsTrue(_userService.AuthenticateUser("testuser", "password123"));
        }

        [TestMethod]
        public void RegisterUser_DuplicateUsername_ShouldFail()
        {
            _userService.RegisterUser("testuser", "password123");
            bool result = _userService.RegisterUser("testuser", "anotherpassword");
            Assert.IsFalse(result);
        }
    }

    [TestClass]
    public class ProductServiceTests
    {
        private static ProductService _productService;
        private static string _connectionString;
        private static string _dbFile;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _connectionString = ConfigurationManager.ConnectionStrings["BakeryDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("Connection string 'BakeryDbConnection' not found in App.config.");
            }

            var builder = new SQLiteConnectionStringBuilder(_connectionString);
            _dbFile = builder.DataSource;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
            DatabaseSetup.InitializeDatabase(_connectionString);
            _productService = new ProductService(_connectionString);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (File.Exists(_dbFile)) File.Delete(_dbFile);
        }

        [TestMethod]
        public void SaveProduct_AddNewProduct_ShouldIncreaseProductCount()
        {
            var initialCount = _productService.GetAllProducts().Count;
            var newProduct = new Product { Name = "New Bread", Price = 50, Weight = 500 };
            _productService.SaveProduct(newProduct);
            var newCount = _productService.GetAllProducts().Count;
            Assert.AreEqual(initialCount + 1, newCount);
        }

        [TestMethod]
        public void SaveProduct_UpdateExistingProduct_ShouldChangeName()
        {
            var product = _productService.GetAllProducts().First();
            string originalName = product.Name;
            product.Name = "Updated Bread Name";
            _productService.SaveProduct(product);

            var updatedProduct = _productService.GetAllProducts().First(p => p.Id == product.Id);
            Assert.AreNotEqual(originalName, updatedProduct.Name);
            Assert.AreEqual("Updated Bread Name", updatedProduct.Name);
        }

        [TestMethod]
        public void DeleteProduct_ShouldDecreaseProductCount()
        {
            var productToDelete = _productService.GetAllProducts().First();
            var initialCount = _productService.GetAllProducts().Count;
            _productService.DeleteProduct(productToDelete.Id);
            var newCount = _productService.GetAllProducts().Count;
            Assert.AreEqual(initialCount - 1, newCount);
        }
    }
}
