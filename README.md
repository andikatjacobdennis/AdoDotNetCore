# AdoNetDemo

![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019-green)
![License](https://img.shields.io/badge/license-MIT-green)

A simple .NET Core console application demonstrating how to perform database operations using ADO.NET with SQL Server.

**For a comprehensive tutorial on ADO.NET, see:**  
[Microsoft Docs: ADO.NET Overview](https://learn.microsoft.com/en-us/dotnet/framework/data/adonet/ado-net-overview)

---

## Table of Contents

- [Prerequisites](#prerequisites)  
- [Setup](#setup)  
- [Build and Run](#build-and-run)  
- [Project Structure](#project-structure)  
- [Notes](#notes)  
- [Contributing](#contributing)  
- [License](#license)  

---

## Prerequisites

- [.NET Core 9.0 SDK](https://dotnet.microsoft.com/en-us/download) or later installed  
- SQL Server instance (2019 or later recommended)  
- Basic knowledge of C# and SQL  

---

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
````

2. Update the connection string in `Program.cs`:

```csharp
string connectionString = "Data Source=.;Initial Catalog=AdoNetDemo;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
```

> This connection string uses Windows Authentication (`Integrated Security=True`) with encryption enabled.

---

## Build and Run

From the project directory, execute the following commands:

```bash
dotnet build
dotnet run
```

---

## Project Structure

* `Program.cs` — Main program demonstrating usage
* `DbConnectionManager.cs` — Manages database connections
* `DbCommandExecutor.cs` — Executes SQL commands
* `EmployeeRepository.cs` — CRUD operations and other queries for the Employees table

---

## Notes

* Always use parameterized queries in production to prevent SQL injection attacks.
* This example uses simple string interpolation for clarity and demonstration purposes only.

---

## Contributing

Contributions are welcome! To contribute:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

Please ensure your code adheres to the existing style and includes relevant tests if applicable.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

*Created with ❤️ for developers learning ADO.NET and .NET Core.*
