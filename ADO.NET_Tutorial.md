# ADO.NET Tutorial

This tutorial will guide you through building a simple .NET Core console application to perform database operations using ADO.NET. We will cover connecting to a database, executing queries, performing CRUD operations, and implementing best practices.

---

## Prerequisites

To get started, you'll need the following:

* Install the .NET Core 9 SDK (if available) or use .NET 6 or later.
* Choose a database. We'll use **SQL Server** in this tutorial.
* Familiarity with **C#** and **SQL** basics.

---

## Step 1: Setting Up the Project

Create a new console application and add the necessary NuGet package.

```bash
dotnet new console -o AdoNetDemo
cd AdoNetDemo

dotnet add package Microsoft.Data.SqlClient
```

> **Note:** If using another database (e.g., PostgreSQL), install its corresponding package like `Npgsql`.

---

## Step 2: Database Setup

Create a database and a sample `Employees` table using the following SQL script:

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
```

---

## Step 3: Connecting to the Database

Create a `DbConnectionManager` class to manage connections:

```csharp
using Microsoft.Data.SqlClient;

public class DbConnectionManager
{
    private readonly string _connectionString;

    public DbConnectionManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}
```

Use it in `Program.cs`:

```csharp
using Microsoft.Data.SqlClient;
using System;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Data Source=.;Initial Catalog=AdoNetDemo;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";
        var dbConnectionManager = new DbConnectionManager(connectionString);

        using var connection = dbConnectionManager.GetConnection();
        connection.Open();
        Console.WriteLine("Connected to the database!");
    }
}
```

---

## Step 4: Executing Queries

Create `DbCommandExecutor` to handle SQL commands:

```csharp
using Microsoft.Data.SqlClient;
using System.Data;

public class DbCommandExecutor
{
    private readonly DbConnectionManager _dbConnectionManager;

    public DbCommandExecutor(DbConnectionManager dbConnectionManager)
    {
        _dbConnectionManager = dbConnectionManager;
    }

    public void ExecuteNonQuery(string query)
    {
        using var connection = _dbConnectionManager.GetConnection();
        connection.Open();

        using var command = new SqlCommand(query, connection);
        command.ExecuteNonQuery();
    }

    public SqlDataReader ExecuteReader(string query)
    {
        var connection = _dbConnectionManager.GetConnection();
        connection.Open();

        var command = new SqlCommand(query, connection);
        return command.ExecuteReader(CommandBehavior.CloseConnection);
    }

    public object ExecuteScalar(string query)
    {
        using var connection = _dbConnectionManager.GetConnection();
        connection.Open();

        using var command = new SqlCommand(query, connection);
        return command.ExecuteScalar();
    }
}
```

---

## Step 5: CRUD Operations

Implement `EmployeeRepository` to perform CRUD on the `Employees` table:

```csharp
using Microsoft.Data.SqlClient;
using System;

public class EmployeeRepository
{
    private readonly DbCommandExecutor _dbCommandExecutor;

    public EmployeeRepository(DbCommandExecutor dbCommandExecutor)
    {
        _dbCommandExecutor = dbCommandExecutor;
    }

    // Create
    public void CreateEmployee(string name, int age)
    {
        string query = $"INSERT INTO Employees (Name, Age) VALUES ('{name}', {age})";
        _dbCommandExecutor.ExecuteNonQuery(query);
    }

    // Read
    public void GetEmployees()
    {
        string query = "SELECT * FROM Employees";
        using var reader = _dbCommandExecutor.ExecuteReader(query);

        while (reader.Read())
        {
            Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
        }
    }

    // Update
    public void UpdateEmployee(int id, string name, int age)
    {
        string query = $"UPDATE Employees SET Name = '{name}', Age = {age} WHERE Id = {id}";
        _dbCommandExecutor.ExecuteNonQuery(query);
    }

    // Delete
    public void DeleteEmployee(int id)
    {
        string query = $"DELETE FROM Employees WHERE Id = {id}";
        _dbCommandExecutor.ExecuteNonQuery(query);
    }
}
```

---

## Step 6: Using Stored Procedures

Create a stored procedure in SQL Server:

```sql
CREATE PROCEDURE sp_GetEmployees
AS
BEGIN
    SELECT * FROM Employees;
END
GO
```

Add this method to `DbCommandExecutor`:

```csharp
public SqlDataReader ExecuteStoredProcedure(string procedureName, SqlParameter[] parameters = null)
{
    var connection = _dbConnectionManager.GetConnection();
    connection.Open();

    var command = new SqlCommand(procedureName, connection);
    command.CommandType = CommandType.StoredProcedure;

    if (parameters != null)
    {
        command.Parameters.AddRange(parameters);
    }

    return command.ExecuteReader(CommandBehavior.CloseConnection);
}
```

Add to `EmployeeRepository`:

```csharp
public void GetEmployeesUsingStoredProcedure()
{
    string procedureName = "sp_GetEmployees";
    using var reader = _dbCommandExecutor.ExecuteStoredProcedure(procedureName);

    while (reader.Read())
    {
        Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
    }
}
```

---

## Step 7: Using DataSet and DataAdapter

Add methods to `EmployeeRepository`:

```csharp
using System.Data;

public DataSet GetEmployeesDataSet()
{
    string query = "SELECT * FROM Employees";
    var dataSet = new DataSet();

    using var connection = _dbConnectionManager.GetConnection();
    connection.Open();

    var adapter = new SqlDataAdapter(query, connection);
    adapter.Fill(dataSet);

    return dataSet;
}

public void GetEmployeesUsingDataSet()
{
    var dataSet = GetEmployeesDataSet();

    foreach (DataRow row in dataSet.Tables[0].Rows)
    {
        Console.WriteLine($"Id: {row["Id"]}, Name: {row["Name"]}, Age: {row["Age"]}");
    }
}
```

---

## Step 8: Asynchronous Operations

Add async methods in `DbCommandExecutor`:

```csharp
using System.Threading.Tasks;

public async Task<SqlDataReader> ExecuteReaderAsync(string query)
{
    var connection = _dbConnectionManager.GetConnection();
    await connection.OpenAsync();

    var command = new SqlCommand(query, connection);
    return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
}

public async Task ExecuteNonQueryAsync(string query)
{
    using var connection = _dbConnectionManager.GetConnection();
    await connection.OpenAsync();

    using var command = new SqlCommand(query, connection);
    await command.ExecuteNonQueryAsync();
}
```

Add async method in `EmployeeRepository`:

```csharp
public async Task GetEmployeesAsync()
{
    string query = "SELECT * FROM Employees";
    using var reader = await _dbCommandExecutor.ExecuteReaderAsync(query);

    while (await reader.ReadAsync())
    {
        Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
    }
}
```

---

## Step 9: Performance Optimization Tips

* **Connection Pooling:** Use `using` statements to ensure connections are closed and returned to the pool.
* **Parameterization:** Always use parameterized queries to avoid SQL injection.
* **Async Operations:** Use async/await for database I/O operations.
* **DataReader vs. DataSet:** Use `SqlDataReader` for fast, forward-only reads; use `DataSet` for in-memory manipulation.

---

## Step 10: Security Best Practices

* Use parameterized queries or stored procedures to prevent SQL injection.
* Encrypt sensitive data in transit and at rest.
* Apply least privilege principle for database access.
* Use robust error handling; avoid exposing raw errors to end users.

---

## Final Example Usage (`Program.cs`)

```csharp
using System;
using System.Data;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "Server=YourServerName;Database=AdoNetDemo;User Id=YourUsername;Password=YourPassword;";

        var dbConnectionManager = new DbConnectionManager(connectionString);
        var dbCommandExecutor = new DbCommandExecutor(dbConnectionManager);
        var employeeRepository = new EmployeeRepository(dbCommandExecutor);

        Console.WriteLine("--- Reading all employees ---");
        employeeRepository.GetEmployees();

        Console.WriteLine("\n--- Creating a new employee ---");
        employeeRepository.CreateEmployee("New Employee", 35);
        employeeRepository.GetEmployees();

        Console.WriteLine("\n--- Updating an employee (Id: 3) ---");
        employeeRepository.UpdateEmployee(3, "Updated Employee", 40);
        employeeRepository.GetEmployees();

        Console.WriteLine("\n--- Deleting an employee (Id: 4) ---");
        employeeRepository.DeleteEmployee(4);
        employeeRepository.GetEmployees();

        Console.WriteLine("\n--- Reading all employees using a stored procedure ---");
        employeeRepository.GetEmployeesUsingStoredProcedure();

        Console.WriteLine("\n--- Reading all employees using a DataSet ---");
        employeeRepository.GetEmployeesUsingDataSet();

        Console.WriteLine("\n--- Reading all employees asynchronously ---");
        await employeeRepository.GetEmployeesAsync();

        Console.WriteLine("\n--- Program complete ---");
    }
}
```
