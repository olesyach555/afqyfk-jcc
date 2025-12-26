using ReservesOfRussia.BLL.Services;
using ReservesOfRussia.DAL;
using ReservesOfRussia.DAL.Models;
using System;
using System.Configuration;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace ReservesOfRussia.Tests
{
    public class ApplicationTests
    {
        private static ReserveService _reserveService;
        private static string _connectionString;
        private static string _dbFile;

        public static void Main(string[] args)
        {
            // --- Setup ---
            _connectionString = ConfigurationManager.ConnectionStrings["ReservesDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("ERROR: Connection string 'ReservesDbConnection' not found in App.config.");
                return;
            }

            var builder = new SQLiteConnectionStringBuilder(_connectionString);
            _dbFile = builder.DataSource;

            // Clean up previous test runs
            if (File.Exists(_dbFile))
            {
                File.Delete(_dbFile);
            }

            DatabaseSetup.InitializeDatabase(_connectionString);
            _reserveService = new ReserveService(_connectionString);

            Console.WriteLine("Running tests...");

            bool test1_success = RunHappyPathTest();
            Console.WriteLine($"Test Scenario 1 (Happy Path): {(test1_success ? "PASSED" : "FAILED")}");

            bool test2_success = RunValidationErrorTest();
            Console.WriteLine($"Test Scenario 2 (Validation Error): {(test2_success ? "PASSED" : "FAILED")}");

            // --- Teardown ---
            if (File.Exists(_dbFile))
            {
                File.Delete(_dbFile);
                Console.WriteLine("\nTest database cleaned up.");
            }

            Console.WriteLine("\nTesting complete. Press any key to exit.");
            Console.ReadKey();
        }

        /// <summary>
        /// Test Scenario 1: Successfully adding a new reserve.
        /// </summary>
        private static bool RunHappyPathTest()
        {
            Console.WriteLine("  Executing Happy Path Test...");
            try
            {
                // 1. Get initial count
                var initialCount = _reserveService.GetAllReserves().Count;

                // 2. Create a valid new reserve
                var newReserve = new Reserve
                {
                    Name = "Test Reserve",
                    Description = "A test description.",
                    Area = 123.45m,
                    FoundationDate = new DateTime(2023, 1, 1),
                    RegionId = 1 // Assuming Region with Id=1 exists
                };

                // 3. Save the reserve
                _reserveService.SaveReserve(newReserve);

                // 4. Verify the reserve was added
                var allReserves = _reserveService.GetAllReserves();
                if (allReserves.Count != initialCount + 1)
                {
                    Console.WriteLine("    FAILURE: Reserve count did not increase.");
                    return false;
                }

                var addedReserve = allReserves.FirstOrDefault(r => r.Name == "Test Reserve");
                if (addedReserve == null)
                {
                    Console.WriteLine("    FAILURE: Could not find the added reserve.");
                    return false;
                }

                // 5. Clean up by deleting the test reserve
                _reserveService.DeleteReserve(addedReserve.Id);
                Console.WriteLine("    Cleanup successful.");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ERROR: An unexpected exception occurred: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test Scenario 2: Attempting to add a reserve with invalid data.
        /// </summary>
        private static bool RunValidationErrorTest()
        {
            Console.WriteLine("  Executing Validation Error Test...");
            try
            {
                // 1. Create an invalid reserve (empty name)
                var invalidReserve = new Reserve
                {
                    Name = "", // Invalid name
                    Area = 100m,
                    RegionId = 1
                };

                // 2. Attempt to save and expect an exception
                _reserveService.SaveReserve(invalidReserve);

                // If we get here, the test failed because no exception was thrown
                Console.WriteLine("    FAILURE: ArgumentException was expected but not thrown.");
                return false;
            }
            catch (ArgumentException argEx)
            {
                // 3. Verify the correct exception was caught
                Console.WriteLine($"    SUCCESS: Caught expected exception: {argEx.Message}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    ERROR: An unexpected exception type was caught: {ex.GetType().Name}");
                return false;
            }
        }
    }
}
