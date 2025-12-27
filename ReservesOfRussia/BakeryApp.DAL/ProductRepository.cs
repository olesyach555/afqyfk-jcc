using BakeryApp.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace BakeryApp.DAL
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Product> GetAllProducts()
        {
            var products = new Dictionary<int, Product>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(
                    @"SELECT p.Id as ProductId, p.Name as ProductName, p.Description, p.Price, p.Weight,
                             pi.IngredientId, i.Name as IngredientName, i.Unit, pi.Quantity
                      FROM Products p
                      LEFT JOIN ProductIngredients pi ON p.Id = pi.ProductId
                      LEFT JOIN Ingredients i ON pi.IngredientId = i.Id", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int productId = Convert.ToInt32(reader["ProductId"]);
                        if (!products.TryGetValue(productId, out var product))
                        {
                            product = new Product
                            {
                                Id = productId,
                                Name = (string)reader["ProductName"],
                                Description = reader["Description"] as string,
                                Price = Convert.ToDecimal(reader["Price"]),
                                Weight = Convert.ToDouble(reader["Weight"])
                            };
                            products.Add(productId, product);
                        }

                        if (reader["IngredientId"] != DBNull.Value)
                        {
                            product.Ingredients.Add(new ProductIngredient
                            {
                                IngredientId = Convert.ToInt32(reader["IngredientId"]),
                                Quantity = Convert.ToDouble(reader["Quantity"]),
                                Ingredient = new Ingredient
                                {
                                    Id = Convert.ToInt32(reader["IngredientId"]),
                                    Name = (string)reader["IngredientName"],
                                    Unit = (string)reader["Unit"]
                                }
                            });
                        }
                    }
                }
            }
            return new List<Product>(products.Values);
        }

        public List<Ingredient> GetAllIngredients()
        {
            var ingredients = new List<Ingredient>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT Id, Name, Unit FROM Ingredients", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ingredients.Add(new Ingredient
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = (string)reader["Name"],
                            Unit = (string)reader["Unit"]
                        });
                    }
                }
            }
            return ingredients;
        }

        public void AddProduct(Product product)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = new SQLiteCommand("INSERT INTO Products (Name, Description, Price, Weight) VALUES (@Name, @Description, @Price, @Weight); SELECT last_insert_rowid();", connection);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Weight", product.Weight);
                    product.Id = (int)(long)command.ExecuteScalar();

                    UpdateProductIngredients(product, connection);

                    transaction.Commit();
                }
            }
        }

        public void UpdateProduct(Product product)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var command = new SQLiteCommand("UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, Weight = @Weight WHERE Id = @Id", connection);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Weight", product.Weight);
                    command.Parameters.AddWithValue("@Id", product.Id);
                    command.ExecuteNonQuery();

                    UpdateProductIngredients(product, connection);

                    transaction.Commit();
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                // Deleting from ProductIngredients is handled by the CASCADE constraint
                var command = new SQLiteCommand("DELETE FROM Products WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", productId);
                command.ExecuteNonQuery();
            }
        }

        private void UpdateProductIngredients(Product product, SQLiteConnection connection)
        {
            // First, remove all existing ingredients for this product
            var deleteCmd = new SQLiteCommand("DELETE FROM ProductIngredients WHERE ProductId = @ProductId", connection);
            deleteCmd.Parameters.AddWithValue("@ProductId", product.Id);
            deleteCmd.ExecuteNonQuery();

            // Then, add the new/updated ingredients
            foreach (var pi in product.Ingredients)
            {
                var insertCmd = new SQLiteCommand("INSERT INTO ProductIngredients (ProductId, IngredientId, Quantity) VALUES (@ProductId, @IngredientId, @Quantity)", connection);
                insertCmd.Parameters.AddWithValue("@ProductId", product.Id);
                insertCmd.Parameters.AddWithValue("@IngredientId", pi.Ingredient.Id);
                insertCmd.Parameters.AddWithValue("@Quantity", pi.Quantity);
                insertCmd.ExecuteNonQuery();
            }
        }
    }
}
