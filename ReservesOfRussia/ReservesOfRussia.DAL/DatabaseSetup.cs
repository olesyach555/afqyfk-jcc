using System;
using System.Data.SQLite;
using System.IO;

namespace ReservesOfRussia.DAL
{
    public static class DatabaseSetup
    {
        public static void InitializeDatabase(string connectionString)
        {
            // Extract the database file path from the connection string
            var builder = new SQLiteConnectionStringBuilder(connectionString);
            var dbFile = builder.DataSource;

            // If the database file already exists, there's nothing to do.
            if (File.Exists(dbFile))
            {
                return;
            }

            // Create the directory if it doesn't exist
            var directory = Path.GetDirectoryName(dbFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create the database file
            SQLiteConnection.CreateFile(dbFile);

            // Connect to the new database and create the schema
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Define the SQL commands to create tables and seed data
                string createScript = @"
                    CREATE TABLE ""Regions"" (
                        ""Id""    INTEGER NOT NULL UNIQUE,
                        ""Name""  TEXT NOT NULL UNIQUE,
                        PRIMARY KEY(""Id"" AUTOINCREMENT)
                    );

                    CREATE TABLE ""Reserves"" (
                        ""Id""    INTEGER NOT NULL UNIQUE,
                        ""Name""  TEXT NOT NULL,
                        ""Description""   TEXT,
                        ""Area""  REAL NOT NULL,
                        ""FoundationDate""    TEXT,
                        ""RegionId""  INTEGER NOT NULL,
                        PRIMARY KEY(""Id"" AUTOINCREMENT),
                        FOREIGN KEY(""RegionId"") REFERENCES ""Regions""(""Id"") ON DELETE CASCADE
                    );

                    INSERT INTO Regions (Name) VALUES ('Красноярский край'), ('Камчатский край'), ('Республика Бурятия');

                    INSERT INTO Reserves (Name, Description, Area, FoundationDate, RegionId)
                    VALUES
                    ('Саяно-Шушенский', 'Расположен в Красноярском крае на левом берегу Енисея.', 3903.68, '1976-03-17', 1),
                    ('Кроноцкий', 'Один из старейших заповедников России, расположен на Камчатке.', 11476.19, '1934-11-01', 2),
                    ('Баргузинский', 'Старейший заповедник России, на берегу озера Байкал.', 3743.22, '1917-01-11', 3);
                ";

                using (var command = new SQLiteCommand(createScript, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
