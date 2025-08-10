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
IF DB_ID('AdoNetDemoDb') IS NULL
BEGIN
    CREATE DATABASE AdoNetDemoDb;
END
GO

USE AdoNetDemoDb;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE Employees (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100),
        Age INT
    );

    INSERT INTO Employees (Name, Age) VALUES ('John Doe', 30);
    INSERT INTO Employees (Name, Age) VALUES ('Jane Doe', 25);
END
GO

IF NOT EXISTS (SELECT * FROM sys.procedures WHERE name = 'sp_GetEmployees')
BEGIN
    EXEC('CREATE PROCEDURE sp_GetEmployees AS BEGIN SELECT * FROM Employees; END');
END
GO
```

---

## Step 3: Connecting to the Database

Create a `DbConnectionManager` class to manage connections:

```csharp
using Microsoft.Data.SqlClient;

namespace AdoNetDemo
{
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
}
```

---

## Step 4: Executing Queries

Create `DbCommandExecutor` to handle SQL commands:

```csharp
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdoNetDemo
{
    public class DbCommandExecutor
    {
        internal readonly DbConnectionManager _dbConnectionManager;

        public DbCommandExecutor(DbConnectionManager dbConnectionManager)
        {
            _dbConnectionManager = dbConnectionManager;
        }

        public void ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using SqlConnection connection = _dbConnectionManager.GetConnection();
                connection.Open();

                using SqlCommand command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        public SqlDataReader ExecuteReader(string query, SqlParameter[]? parameters = null)
        {
            SqlConnection? connection = null;
            try
            {
                connection = _dbConnectionManager.GetConnection();
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                connection?.Close();
                throw; // Re-throw the exception to be handled by the caller
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                connection?.Close();
                throw; // Re-throw the exception to be handled by the caller
            }
        }

        public object ExecuteScalar(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using SqlConnection connection = _dbConnectionManager.GetConnection();
                connection.Open();

                using SqlCommand command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                return DBNull.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return DBNull.Value;
            }
        }

        public SqlDataReader ExecuteStoredProcedure(string procedureName, SqlParameter[]? parameters = null)
        {
            SqlConnection? connection = null;
            try
            {
                connection = _dbConnectionManager.GetConnection();
                connection.Open();

                SqlCommand command = new SqlCommand(procedureName, connection);
                command.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                return command.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                connection?.Close();
                throw; // Re-throw the exception
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                connection?.Close();
                throw; // Re-throw the exception
            }
        }

        public async Task<SqlDataReader> ExecuteReaderAsync(string query, SqlParameter[]? parameters = null)
        {
            SqlConnection? connection = null;
            try
            {
                connection = _dbConnectionManager.GetConnection();
                await connection.OpenAsync();

                SqlCommand command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
                connection?.Close();
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                connection?.Close();
                throw;
            }
        }

        public async Task ExecuteNonQueryAsync(string query, SqlParameter[]? parameters = null)
        {
            try
            {
                using SqlConnection connection = _dbConnectionManager.GetConnection();
                await connection.OpenAsync();

                using SqlCommand command = new SqlCommand(query, connection);
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                await command.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
```

---

## Step 5: CRUD Operations

Implement `EmployeeRepository` to perform CRUD on the `Employees` table:

```csharp
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdoNetDemo
{
    public class EmployeeRepository
    {
        private readonly DbCommandExecutor _dbCommandExecutor;

        public EmployeeRepository(DbCommandExecutor dbCommandExecutor)
        {
            _dbCommandExecutor = dbCommandExecutor;
        }

        public bool DatabaseExists(string databaseName)
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_dbCommandExecutor._dbConnectionManager.GetConnection().ConnectionString)
                {
                    InitialCatalog = "master"
                };

                string query = "SELECT database_id FROM sys.databases WHERE Name = @dbName";
                SqlParameter[] parameters = {
                    new SqlParameter("@dbName", SqlDbType.NVarChar) { Value = databaseName }
                };

                using SqlConnection connection = new SqlConnection(builder.ConnectionString);
                connection.Open();
                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddRange(parameters);

                object result = command.ExecuteScalar();
                return result != null && result != DBNull.Value;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database existence check failed: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred during database check: {ex.Message}");
                return false;
            }
        }

        public void CreateEmployee(string name, int age)
        {
            string query = "INSERT INTO Employees (Name, Age) VALUES (@name, @age)";
            SqlParameter[] parameters = {
                new SqlParameter("@name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@age", SqlDbType.Int) { Value = age }
            };
            _dbCommandExecutor.ExecuteNonQuery(query, parameters);
        }

        public void GetEmployees()
        {
            string query = "SELECT * FROM Employees";
            try
            {
                using SqlDataReader reader = _dbCommandExecutor.ExecuteReader(query);
                while (reader.Read())
                {
                    Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
                }
            }
            catch (Exception ex)
            {
                // The error message is handled in DbCommandExecutor, here we just catch any propagated errors
                Console.WriteLine($"Failed to retrieve employees: {ex.Message}");
            }
        }

        public void UpdateEmployee(int id, string name, int age)
        {
            string query = "UPDATE Employees SET Name = @name, Age = @age WHERE Id = @id";
            SqlParameter[] parameters = {
                new SqlParameter("@id", SqlDbType.Int) { Value = id },
                new SqlParameter("@name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@age", SqlDbType.Int) { Value = age }
            };
            _dbCommandExecutor.ExecuteNonQuery(query, parameters);
        }

        public void DeleteEmployee(int id)
        {
            string query = "DELETE FROM Employees WHERE Id = @id";
            SqlParameter[] parameters = {
                new SqlParameter("@id", SqlDbType.Int) { Value = id }
            };
            _dbCommandExecutor.ExecuteNonQuery(query, parameters);
        }

        public void GetEmployeesUsingStoredProcedure()
        {
            string procedureName = "sp_GetEmployees";
            try
            {
                using SqlDataReader reader = _dbCommandExecutor.ExecuteStoredProcedure(procedureName);
                while (reader.Read())
                {
                    Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve employees from stored procedure: {ex.Message}");
            }
        }

        public DataSet GetEmployeesDataSet()
        {
            string query = "SELECT * FROM Employees";
            DataSet dataSet = new DataSet();
            try
            {
                using SqlConnection connection = _dbCommandExecutor._dbConnectionManager.GetConnection();
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dataSet);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Database error filling DataSet: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred filling DataSet: {ex.Message}");
            }
            return dataSet;
        }

        public void GetEmployeesUsingDataSet()
        {
            DataSet dataSet = GetEmployeesDataSet();

            if (dataSet.Tables.Count > 0)
            {
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    Console.WriteLine($"Id: {row["Id"]}, Name: {row["Name"]}, Age: {row["Age"]}");
                }
            }
        }

        public async Task GetEmployeesAsync()
        {
            string query = "SELECT * FROM Employees";
            try
            {
                using SqlDataReader reader = await _dbCommandExecutor.ExecuteReaderAsync(query);
                while (await reader.ReadAsync())
                {
                    Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve employees asynchronously: {ex.Message}");
            }
        }
    }
}
```

---

## Step 6: Performance Optimization Tips

* **Connection Pooling:** Use `using` statements to ensure connections are closed and returned to the pool.
* **Parameterization:** Always use parameterized queries to avoid SQL injection.
* **Async Operations:** Use async/await for database I/O operations.
* **DataReader vs. DataSet:** Use `SqlDataReader` for fast, forward-only reads; use `DataSet` for in-memory manipulation.

---

## Step 7: Security Best Practices

* Use parameterized queries or stored procedures to prevent SQL injection.
* Encrypt sensitive data in transit and at rest.
* Apply least privilege principle for database access.
* Use robust error handling; avoid exposing raw errors to end users.

---

## Final Example Usage (`Program.cs`)

```csharp
namespace AdoNetDemo
{
    class Program
    {
        static async Task Main()
        {
            // Note: The serverName "." typically refers to the local SQL Server Express instance.
            // Adjust this if your SQL Server instance has a different name.
            string serverName = ".";
            string databaseName = "AdoNetDemoDb";
            string connectionString = $"Data Source={serverName};Initial Catalog={databaseName};Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            DbConnectionManager dbConnectionManager = new DbConnectionManager(connectionString);
            DbCommandExecutor dbCommandExecutor = new DbCommandExecutor(dbConnectionManager);
            EmployeeRepository employeeRepository = new EmployeeRepository(dbCommandExecutor);

            try
            {
                if (employeeRepository.DatabaseExists(databaseName))
                {
                    Console.WriteLine("Database exists.");

                    Console.WriteLine("--- Reading all employees ---");
                    employeeRepository.GetEmployees();

                    Console.WriteLine("\n--- Creating a new employee ---");
                    // Using a parameterized query for a secure INSERT operation
                    employeeRepository.CreateEmployee("New Employee", 35);
                    employeeRepository.GetEmployees();

                    Console.WriteLine("\n--- Updating an employee (Id: 3) ---");
                    // Using a parameterized query for a secure UPDATE operation
                    employeeRepository.UpdateEmployee(3, "Updated Employee", 40);
                    employeeRepository.GetEmployees();

                    Console.WriteLine("\n--- Deleting an employee (Id: 4) ---");
                    // Using a parameterized query for a secure DELETE operation
                    employeeRepository.DeleteEmployee(4);
                    employeeRepository.GetEmployees();

                    Console.WriteLine("\n--- Reading all employees using a stored procedure ---");
                    employeeRepository.GetEmployeesUsingStoredProcedure();

                    Console.WriteLine("\n--- Reading all employees using a DataSet ---");
                    employeeRepository.GetEmployeesUsingDataSet();

                    Console.WriteLine("\n--- Reading all employees asynchronously ---");
                    await employeeRepository.GetEmployeesAsync();
                }
                else
                {
                    Console.WriteLine("Database does not exist. Please run the setup script first.");
                }
            }
            catch (Exception ex)
            {
                // This catch block handles any unhandled exceptions from the repository.
                // In a real application, you would log this error.
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            Console.WriteLine("\n--- Program complete ---");
        }
    }
}
```
