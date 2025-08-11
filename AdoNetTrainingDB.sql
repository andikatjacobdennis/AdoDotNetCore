-- 1. Create Database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AdoNetTrainingDB')
BEGIN
    CREATE DATABASE AdoNetTrainingDB;
    PRINT 'Database AdoNetTrainingDB created.';
END
ELSE
BEGIN
    PRINT 'Database AdoNetTrainingDB already exists.';
END
GO

USE AdoNetTrainingDB;
GO

-- 2. Create Table: YourTable
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='YourTable' AND xtype='U')
BEGIN
    CREATE TABLE YourTable (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL
    );
    PRINT 'Table YourTable created.';
END
ELSE
BEGIN
    PRINT 'Table YourTable already exists.';
END
GO

-- 3. Create Table: LargeTable
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LargeTable' AND xtype='U')
BEGIN
    CREATE TABLE LargeTable (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        DataValue NVARCHAR(255) NOT NULL
    );
    PRINT 'Table LargeTable created.';
END
ELSE
BEGIN
    PRINT 'Table LargeTable already exists.';
END
GO

-- 4. Insert Sample Data into YourTable only if empty
IF NOT EXISTS (SELECT 1 FROM YourTable)
BEGIN
    INSERT INTO YourTable (Name) VALUES
    ('Alice'),
    ('Bob'),
    ('Charlie'),
    ('David'),
    ('Eva');
    PRINT 'Sample data inserted into YourTable.';
END
ELSE
BEGIN
    PRINT 'YourTable already has data.';
END
GO

-- 5. Insert Bulk Data into LargeTable only if empty
IF NOT EXISTS (SELECT 1 FROM LargeTable)
BEGIN
    DECLARE @i INT = 1;
    WHILE @i <= 1000
    BEGIN
        INSERT INTO LargeTable (DataValue) VALUES ('Value ' + CAST(@i AS NVARCHAR(10)));
        SET @i += 1;
    END
    PRINT 'Sample bulk data inserted into LargeTable.';
END
ELSE
BEGIN
    PRINT 'LargeTable already has data.';
END
GO

-- 6. Create Stored Procedure if it doesn't exist
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MyStoredProc' AND xtype='P')
BEGIN
    EXEC('
    CREATE PROCEDURE MyStoredProc
        @Param1 NVARCHAR(100)
    AS
    BEGIN
        SET NOCOUNT ON;
        INSERT INTO YourTable (Name) VALUES (@Param1);
        SELECT ''Inserted '' + @Param1 AS ResultMessage;
    END
    ');
    PRINT 'Stored procedure MyStoredProc created.';
END
ELSE
BEGIN
    PRINT 'Stored procedure MyStoredProc already exists.';
END
GO
