using BakeryApp.DAL;
using BakeryApp.DAL.Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BakeryApp.BLL.Services
{
    public class UserService
    {
        private readonly UserRepository _repository;
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 10000;

        public UserService(string connectionString)
        {
            _repository = new UserRepository(connectionString);
        }

        public bool RegisterUser(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Username and password cannot be empty.");
            }

            if (_repository.GetUserByUsername(username) != null)
            {
                return false;
            }

            // Generate a salt
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt
            var hash = HashPassword(password, salt);

            var user = new User
            {
                Username = username,
                PasswordHash = Convert.ToBase64String(hash),
                PasswordSalt = Convert.ToBase64String(salt)
            };

            _repository.AddUser(user);
            return true;
        }

        public bool AuthenticateUser(string username, string password)
        {
            var user = _repository.GetUserByUsername(username);
            if (user == null)
            {
                return false;
            }

            // Hash the provided password with the stored salt
            var salt = Convert.FromBase64String(user.PasswordSalt);
            var hash = HashPassword(password, salt);
            var hashToCompare = Convert.ToBase64String(hash);


            // Compare the hashes
            return user.PasswordHash == hashToCompare;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return rfc2898DeriveBytes.GetBytes(KeySize);
            }
        }
    }
}
