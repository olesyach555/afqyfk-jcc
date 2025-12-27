using BakeryApp.DAL.Models;
using System;
using System.Data.SQLite;

namespace BakeryApp.DAL
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUser(User user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("INSERT INTO Users (Username, PasswordHash, PasswordSalt) VALUES (@Username, @PasswordHash, @PasswordSalt)", connection);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
                command.ExecuteNonQuery();
            }
        }

        public User GetUserByUsername(string username)
        {
            User user = null;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                var command = new SQLiteCommand("SELECT Id, Username, PasswordHash, PasswordSalt FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Username = (string)reader["Username"],
                            PasswordHash = (string)reader["PasswordHash"],
                            PasswordSalt = (string)reader["PasswordSalt"]
                        };
                    }
                }
            }
            return user;
        }
    }
}
