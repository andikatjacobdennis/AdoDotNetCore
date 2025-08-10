# Ado Net Demo

![.NET](https://img.shields.io/badge/.NET-9.0-blue)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019-green)
![License](https://img.shields.io/badge/license-MIT-green)

A simple .NET Core console application demonstrating database operations using ADO.NET with SQL Server.

For a detailed step-by-step guide, please see the full tutorial:  
[ADO.NET Tutorial](ADO.NET_Tutorial.md)

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

---

## Build and Run

From the project directory:

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
* `ADO.NET_Tutorial.md` — Detailed tutorial with full code examples and best practices

---

## Notes

* Always use parameterized queries or stored procedures in production to prevent SQL injection.
* The provided examples use string interpolation for clarity; avoid this in real applications.

---

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a new feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m "Add feature"`)
4. Push to your branch (`git push origin feature/YourFeature`)
5. Open a Pull Request for review

Please ensure your code follows existing style conventions and includes tests where appropriate.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

*Made with ❤️ for developers exploring ADO.NET and .NET Core.*
