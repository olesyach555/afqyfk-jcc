-- Drop existing tables if they exist to ensure a clean setup
IF OBJECT_ID('dbo.Reserves', 'U') IS NOT NULL
    DROP TABLE dbo.Reserves;
GO

IF OBJECT_ID('dbo.Regions', 'U') IS NOT NULL
    DROP TABLE dbo.Regions;
GO

-- Create table for regions
CREATE TABLE Regions (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Create table for reserves
CREATE TABLE Reserves (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Area DECIMAL(18, 2) NOT NULL, -- Area in sq. km
    FoundationDate DATE NULL,
    RegionId INT NOT NULL,

    -- Create a foreign key to link with the Regions table
    CONSTRAINT FK_Reserves_Regions FOREIGN KEY (RegionId) REFERENCES Regions(Id) ON DELETE CASCADE
);
GO

-- Add some test data
INSERT INTO Regions (Name) VALUES ('Krasnoyarsk Krai'), ('Kamchatka Krai'), ('Buryatia Republic');
GO

INSERT INTO Reserves (Name, Description, Area, FoundationDate, RegionId)
VALUES
('Sayano-Shushensky', 'Located in Krasnoyarsk Krai on the left bank of the Yenisei River.', 3903.68, '1976-03-17', 1),
('Kronotsky', 'One of the oldest reserves in Russia, located in Kamchatka.', 11476.19, '1934-11-01', 2),
('Barguzinsky', 'The oldest reserve in Russia, on the shore of Lake Bail.', 3743.22, '1917-01-11', 3);
GO

PRINT 'Database setup complete. Tables created and seeded with initial data.';
