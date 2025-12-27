using System;
using System.Data.SQLite;
using System.IO;

namespace BakeryApp.DAL
{
    public static class DatabaseSetup
    {
        public static void InitializeDatabase(string connectionString)
        {
            var builder = new SQLiteConnectionStringBuilder(connectionString);
            var dbFile = builder.DataSource;

            if (File.Exists(dbFile))
            {
                return;
            }

            var directory = Path.GetDirectoryName(dbFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            SQLiteConnection.CreateFile(dbFile);

            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                string createScript = @"
                    -- Roles Table: Stores user roles (e.g., 'Admin', 'User')
                    CREATE TABLE ""Roles"" (
                        ""Id""    INTEGER NOT NULL UNIQUE,
                        ""Name""  TEXT NOT NULL UNIQUE,
                        PRIMARY KEY(""Id"" AUTOINCREMENT)
                    );

                    -- Users Table: Stores user credentials for login
                    CREATE TABLE ""Users"" (
                        ""Id""            INTEGER NOT NULL UNIQUE,
                        ""Username""      TEXT NOT NULL UNIQUE,
                        ""PasswordHash""  TEXT NOT NULL,
                        ""PasswordSalt""  TEXT NOT NULL,
                        PRIMARY KEY(""Id"" AUTOINCREMENT)
                    );

                    -- UserRoles Junction Table: Links users to roles (many-to-many)
                    CREATE TABLE ""UserRoles"" (
                        ""UserId""    INTEGER NOT NULL,
                        ""RoleId""    INTEGER NOT NULL,
                        PRIMARY KEY(""UserId"", ""RoleId""),
                        FOREIGN KEY(""UserId"") REFERENCES ""Users""(""Id"") ON DELETE CASCADE,
                        FOREIGN KEY(""RoleId"") REFERENCES ""Roles""(""Id"") ON DELETE CASCADE
                    );

                    -- Products Table: Stores bakery products
                    CREATE TABLE ""Products"" (
                        ""Id""            INTEGER NOT NULL UNIQUE,
                        ""Name""          TEXT NOT NULL,
                        ""Description""   TEXT,
                        ""Price""         REAL NOT NULL,
                        ""Weight""        REAL NOT NULL,
                        PRIMARY KEY(""Id"" AUTOINCREMENT)
                    );

                    -- Ingredients Table: Stores ingredients used in products
                    CREATE TABLE ""Ingredients"" (
                        ""Id""    INTEGER NOT NULL UNIQUE,
                        ""Name""  TEXT NOT NULL UNIQUE,
                        ""Unit""  TEXT NOT NULL, -- (e.g., 'гр', 'мл', 'шт')
                        PRIMARY KEY(""Id"" AUTOINCREMENT)
                    );

                    -- ProductIngredients Junction Table: Links products to ingredients with quantities
                    CREATE TABLE ""ProductIngredients"" (
                        ""ProductId""     INTEGER NOT NULL,
                        ""IngredientId""  INTEGER NOT NULL,
                        ""Quantity""      REAL NOT NULL,
                        PRIMARY KEY(""ProductId"", ""IngredientId""),
                        FOREIGN KEY(""ProductId"") REFERENCES ""Products""(""Id"") ON DELETE CASCADE,
                        FOREIGN KEY(""IngredientId"") REFERENCES ""Ingredients""(""Id"") ON DELETE CASCADE
                    );

                    -- Seed Data
                    INSERT INTO Roles (Name) VALUES ('Администратор'), ('Пользователь');

                    INSERT INTO Ingredients (Name, Unit) VALUES
                    ('Мука пшеничная в/с', 'гр'), ('Сахар', 'гр'), ('Соль', 'гр'),
                    ('Дрожжи', 'гр'), ('Вода', 'мл'), ('Масло сливочное', 'гр'), ('Яйцо', 'шт');

                    INSERT INTO Products (Name, Description, Price, Weight) VALUES
                    ('Хлеб ""Бородинский""', 'Классический ржано-пшеничный хлеб с тмином.', 45.50, 500.0),
                    ('Батон ""Нарезной""', 'Пшеничный батон для ежедневного употребления.', 38.00, 400.0);

                    -- Seed a recipe for 'Батон ""Нарезной""' (ProductId = 2)
                    INSERT INTO ProductIngredients (ProductId, IngredientId, Quantity) VALUES
                    (2, 1, 300.0), -- Мука
                    (2, 2, 20.0),  -- Сахар
                    (2, 3, 5.0),   -- Соль
                    (2, 4, 10.0),  -- Дрожжи
                    (2, 5, 150.0), -- Вода
                    (2, 6, 25.0);  -- Масло
                ";

                using (var command = new SQLiteCommand(createScript, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
