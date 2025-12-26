using ReservesOfRussia.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace ReservesOfRussia.DAL
{
    public class ReserveRepository
    {
        private readonly string _connectionString;

        public ReserveRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Reserve> GetAllReserves()
        {
            var reserves = new List<Reserve>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT r.Id, r.Name, r.Description, r.Area, r.FoundationDate, r.RegionId, rg.Name as RegionName FROM Reserves r JOIN Regions rg ON r.RegionId = rg.Id", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reserves.Add(new Reserve
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Description = reader["Description"] as string,
                            Area = (decimal)reader["Area"],
                            FoundationDate = reader["FoundationDate"] as DateTime?,
                            RegionId = (int)reader["RegionId"],
                            Region = new Region { Id = (int)reader["RegionId"], Name = (string)reader["RegionName"] }
                        });
                    }
                }
            }
            return reserves;
        }

        public List<Region> GetAllRegions()
        {
            var regions = new List<Region>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Id, Name FROM Regions", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        regions.Add(new Region
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"]
                        });
                    }
                }
            }
            return regions;
        }

        public void AddReserve(Reserve reserve)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Reserves (Name, Description, Area, FoundationDate, RegionId) VALUES (@Name, @Description, @Area, @FoundationDate, @RegionId)", connection);
                command.Parameters.AddWithValue("@Name", reserve.Name);
                command.Parameters.AddWithValue("@Description", (object)reserve.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Area", reserve.Area);
                command.Parameters.AddWithValue("@FoundationDate", (object)reserve.FoundationDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@RegionId", reserve.RegionId);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateReserve(Reserve reserve)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Reserves SET Name = @Name, Description = @Description, Area = @Area, FoundationDate = @FoundationDate, RegionId = @RegionId WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Name", reserve.Name);
                command.Parameters.AddWithValue("@Description", (object)reserve.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@Area", reserve.Area);
                command.Parameters.AddWithValue("@FoundationDate", (object)reserve.FoundationDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@RegionId", reserve.RegionId);
                command.Parameters.AddWithValue("@Id", reserve.Id);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteReserve(int reserveId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM Reserves WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", reserveId);
                command.ExecuteNonQuery();
            }
        }
    }
}
