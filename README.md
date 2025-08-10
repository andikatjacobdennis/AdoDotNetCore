# AdoNetDemo

This is a simple .NET Core console application demonstrating how to perform database operations using ADO.NET with SQL Server.

## Prerequisites

- .NET Core 9.0 or later SDK installed
- SQL Server instance
- Basic knowledge of C# and SQL

## Setup

1. Create the database and table in SQL Server:

```sql
CREATE DATABASE AdoNetDemo;
GO

USE AdoNetDemo;
GO

CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100),
    Age INT
);
GO

INSERT INTO Employees (Name, Age) VALUES ('John Doe', 30);
INSERT INTO Employees (Name, Age) VALUES ('Jane Doe', 25);
GO

CREATE PROCEDURE sp_GetEmployees
AS
BEGIN
    SELECT * FROM Employees;
END
GO
```

2. Update the connection string in `Program.cs`:

```csharp
string connectionString = "Server=YourServerName;Database=AdoNetDemo;User Id=YourUsername;Password=YourPassword;";
```

Replace `YourServerName`, `YourUsername`, and `YourPassword` with your SQL Server details.

## Build and Run

From the project directory:

```bash
dotnet build
dotnet run
```

---

## Project Structure

- `Program.cs` - Main program demonstrating usage
- `DbConnectionManager.cs` - Manages database connections
- `DbCommandExecutor.cs` - Executes SQL commands
- `EmployeeRepository.cs` - CRUD operations and other queries for Employees table

---

## Notes

- Always use parameterized queries in production code to prevent SQL injection.
- This example uses simple string interpolation for clarity.
