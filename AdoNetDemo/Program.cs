using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Runtime.Versioning;
using System.Text.Json;

namespace AdoNetDemo
{
    class Program
    {
        private static readonly string connectionString =
            "Server=.;Database=AdoNetTrainingDB;Trusted_Connection=True;TrustServerCertificate=True;";
        private static readonly string accessConnectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=SampleAccessDB.accdb;Persist Security Info=False;";

        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        static void Main()
        {
            // Handle graceful exit on Ctrl+C
            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("\nExiting...");
                e.Cancel = true;
                Environment.Exit(0);
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== ADO.NET Training Menu ===");
                Console.WriteLine("1. Introduction");
                Console.WriteLine("2. Environment Setup");
                Console.WriteLine("3. Disconnected Architecture");
                Console.WriteLine("4. Understanding DataSet");
                Console.WriteLine("5. Serializing DataSet");
                Console.WriteLine("6. Connected Architecture");
                Console.WriteLine("7. Establishing Connection");
                Console.WriteLine("8. Executing Commands");
                Console.WriteLine("9. CRUD Operations");
                Console.WriteLine("10. Reading Bulk Data");
                Console.WriteLine("11. SQL Injection Example");
                Console.WriteLine("12. Parameterized Commands (Recommended)");
                Console.WriteLine("13. Executing Stored Procedures");
                Console.WriteLine("14. SQL Command Builder");
                Console.WriteLine("15. SQL Bulk Copy");
                Console.WriteLine("16. Transactions");
                Console.WriteLine("17. Connect to MS Access Database");
                Console.WriteLine("18. XML Data Read/Write");
                Console.WriteLine("19. Disconnected Update Back to SQL Server");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");

                string? choice = Console.ReadLine();
                Console.Clear();

                try
                {
                    switch (choice)
                    {
                        case "1": Introduction(); break;
                        case "2": EnvironmentSetup(); break;
                        case "3": DisconnectedArchitecture(); break;
                        case "4": UnderstandingDataSet(); break;
                        case "5": SerializingDataSet(); break;
                        case "6": ConnectedArchitecture(); break;
                        case "7": EstablishingConnection(); break;
                        case "8": ExecutingCommands(); break;
                        case "9": CrudOperations(); break;
                        case "10": ReadingBulkData(); break;
                        case "11": SqlInjectionExample(); break;
                        case "12": ParameterizedCommands(); break;
                        case "13": ExecutingProcedures(); break;
                        case "14": SqlCommandBuilderDemo(); break;
                        case "15": SqlBulkCopyDemo(); break;
                        case "16": TransactionsDemo(); break;
                        case "17":
                            if (OperatingSystem.IsWindows())
                                MsAccessConnection();
                            else
                                Console.WriteLine("Access DB operations are only supported on Windows.");
                            break;
                        case "18": XmlDataReadWrite(); break;
                        case "19": DisconnectedUpdateBackToSql(); break;
                        case "0": return;
                        default: Console.WriteLine("Invalid choice."); break;
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"[SQL ERROR] {ex.Message}");
                }
                catch (OleDbException ex)
                {
                    Console.WriteLine($"[Access DB ERROR] {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] An unexpected error occurred: {ex.Message}");
                }

                Console.WriteLine("\nPress Enter to return to menu...");
                Console.ReadLine();
            }
        }

        static void Introduction()
        {
            Console.WriteLine("ADO.NET is a set of classes for accessing data sources from .NET.");
            Console.WriteLine("It supports **Connected** & **Disconnected** models using components like `SqlConnection`, `SqlCommand`, `DataSet`, and `DataAdapter`.");
        }

        static void EnvironmentSetup()
        {
            Console.WriteLine("### Environment Setup Steps");
            Console.WriteLine("1. Install **Visual Studio 2022** or **Visual Studio Code**.");
            Console.WriteLine("   - VS: https://visualstudio.microsoft.com/vs/");
            Console.WriteLine("   - VS Code: https://code.visualstudio.com/");
            Console.WriteLine();
            Console.WriteLine("2. Install **.NET 9 SDK**:");
            Console.WriteLine("   - https://dotnet.microsoft.com/en-us/download/dotnet/9.0");
            Console.WriteLine();
            Console.WriteLine("3. Create a Console Project:");
            Console.WriteLine("   `dotnet new console -n AdoNetDemo -f net9.0`");
            Console.WriteLine();
            Console.WriteLine("4. Install necessary NuGet packages:");
            Console.WriteLine("   - **Microsoft.Data.SqlClient** (for SQL Server)");
            Console.WriteLine("   `dotnet add package Microsoft.Data.SqlClient`");
            Console.WriteLine("   - **System.Data.OleDb** (for MS Access, only on Windows)");
            Console.WriteLine("   `dotnet add package System.Data.OleDb`");
        }

        static void DisconnectedArchitecture()
        {
            Console.WriteLine("--- Disconnected Architecture ---");
            Console.WriteLine("Fetching data into a DataSet which operates offline...");

            try
            {
                using var conn = new SqlConnection(connectionString);
                using var adapter = new SqlDataAdapter("SELECT TOP 5 Id, Name FROM YourTable", conn);
                var dataSet = new DataSet();

                int rowsFetched = adapter.Fill(dataSet, "YourTable");

                if (rowsFetched > 0)
                {
                    var table = dataSet.Tables["YourTable"];
                    if (table != null && table.Rows.Count > 0)
                    {
                        Console.WriteLine($"Data loaded into DataSet ({table.Rows.Count} rows).");
                        foreach (DataRow row in table.Rows)
                        {
                            Console.WriteLine($"ID: {row["Id"]}, Name: {row["Name"]}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("? No data returned from 'YourTable'.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[SQL Error] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }

        static void UnderstandingDataSet()
        {
            var ds = new DataSet("MyDataSet");
            var dt = new DataTable("People");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");
            ds.Tables.Add(dt);

            Console.WriteLine($"DataSet Name: **{ds.DataSetName}**");
            Console.WriteLine($"Number of Tables: **{ds.Tables.Count}**");
            Console.WriteLine($"Table Name: **{ds.Tables[0].TableName}**");
            Console.WriteLine($"Number of Rows: **{ds.Tables[0].Rows.Count}**");
            Console.WriteLine($"Data from DataTable:");
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Console.WriteLine($"{row["Id"]} - {row["Name"]}");
            }
        }

        static void SerializingDataSet()
        {
            Console.WriteLine("--- Serializing DataSet ---");

            try
            {
                // Build the DataSet
                var ds = new DataSet("PeopleDS");
                var dt = new DataTable("People");
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Rows.Add(1, "Alice");
                dt.Rows.Add(2, "Bob");
                ds.Tables.Add(dt);

                // Write full DataSet to XML
                string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "people.xml");
                ds.WriteXml(xmlFilePath, XmlWriteMode.WriteSchema);
                Console.WriteLine($"Serialized to XML. File created at: {xmlFilePath}");

                // Prepare data for JSON serialization
                var peopleList = new List<object>();
                var table = ds.Tables["People"];
                if (table != null && table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        peopleList.Add(new
                        {
                            Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                            Name = row["Name"]?.ToString() ?? string.Empty
                        });
                    }
                }
                else
                {
                    Console.WriteLine("No data found in 'People' table for JSON serialization.");
                }

                // Serialize to JSON
                string json = JsonSerializer.Serialize(peopleList, _jsonOptions);
                string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "people.json");
                File.WriteAllText(jsonFilePath, json);

                Console.WriteLine($"Serialized to JSON. File created at: {jsonFilePath}");
                Console.WriteLine("\nJSON Output:");
                Console.WriteLine(json);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"[File Error] {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }

        static void ConnectedArchitecture()
        {
            Console.WriteLine("--- Connected Architecture ---");
            Console.WriteLine("Streaming data directly from the database, connection remains open.");

            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand("SELECT TOP 5 Id, Name FROM YourTable", conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["Id"]} - {reader["Name"]}");
            }
        }

        static void EstablishingConnection()
        {
            Console.WriteLine("--- Establishing Connection ---");
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connection established successfully! The connection will now be closed by the `using` block.");
        }

        static void ExecutingCommands()
        {
            Console.WriteLine("--- Executing Commands ---");
            Console.WriteLine("Demonstrating `ExecuteScalar` to retrieve a single value.");
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT COUNT(*) FROM YourTable", conn);
            int count = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Total Rows in YourTable: {count}");
        }

        static void CrudOperations()
        {
            Console.WriteLine("--- CRUD Operations ---");

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                // CREATE (parameterized)
                Console.WriteLine("\n1. Inserting a new record using a parameterized command.");
                using (var insertCmd = new SqlCommand("INSERT INTO YourTable (Name) VALUES (@name)", conn))
                {
                    insertCmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = "SecureName";
                    int rowsInserted = insertCmd.ExecuteNonQuery();
                    Console.WriteLine($"Inserted {rowsInserted} record(s).");
                }

                // READ
                Console.WriteLine("\n2. Reading the last inserted record.");
                using (var selectCmd = new SqlCommand("SELECT TOP 1 Id, Name FROM YourTable ORDER BY Id DESC", conn))
                using (var reader = selectCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"Last Inserted Record ? Id: {reader["Id"]}, Name: {reader["Name"]}");
                    }
                    else
                    {
                        Console.WriteLine("No records found.");
                    }
                }

                // UPDATE (parameterized)
                Console.WriteLine("\n3. Updating the last inserted record.");
                string updatedName = $"UpdatedName_{DateTime.Now:yyyyMMdd_HHmmssfff}";
                using var updateCmd = new SqlCommand("UPDATE YourTable SET Name = @newName WHERE Id = (SELECT TOP 1 Id FROM YourTable ORDER BY Id DESC)", conn);
                updateCmd.Parameters.Add("@newName", SqlDbType.NVarChar, 100).Value = updatedName;
                int rowsUpdated = updateCmd.ExecuteNonQuery();
                Console.WriteLine(rowsUpdated > 0
                    ? $"Updated {rowsUpdated} record(s) to '{updatedName}'."
                    : "No records updated.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[SQL Error] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }

        static void ReadingBulkData()
        {
            Console.WriteLine("--- Reading Bulk Data ---");
            Console.WriteLine("This demonstrates a `SqlDataReader` for efficient, forward-only reading of large datasets.");
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var cmd = new SqlCommand("SELECT * FROM LargeTable", conn);
            using var reader = cmd.ExecuteReader();

            Console.WriteLine("Reading data line by line (first 5 rows shown):");
            int count = 0;
            while (reader.Read() && count < 5)
            {
                Console.WriteLine($"{reader[0]} - {reader[1]}...");
                count++;
            }
            Console.WriteLine($"...and so on. Total fields: {reader.FieldCount}");
        }

        static void SqlInjectionExample()
        {
            Console.WriteLine("--- DANGER: SQL Injection Example ---");
            Console.WriteLine("This is a **vulnerable approach** and should **NEVER** be used in production.");
            Console.WriteLine("Assume a user inputs `1 OR 1=1; --`");
            string userInput = "1 OR 1=1; --";
            string vulnerableQuery = "SELECT * FROM YourTable WHERE Id = " + userInput;

            Console.WriteLine($"Vulnerable Query: `{vulnerableQuery}`");
            Console.WriteLine("This query would return all rows, bypassing the intended filter.");
            Console.WriteLine("\nInstead, always use parameterized queries to prevent this attack.");
        }

        static void ParameterizedCommands()
        {
            Console.WriteLine("--- Parameterized Commands ---");
            Console.WriteLine("The recommended way to pass values to queries safely.");

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                // Demonstrating a safe parameterized query
                using var cmd = new SqlCommand("SELECT Id, Name FROM YourTable WHERE Id = @id", conn);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = 1; // Example: replace with user input

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"Found record for Id 1: {reader["Name"]}");
                }
                else
                {
                    Console.WriteLine("No record found with Id 1.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[SQL Error] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }

        static void ExecutingProcedures()
        {
            Console.WriteLine("--- Executing Stored Procedures ---");

            try
            {
                using var conn = new SqlConnection(connectionString);
                conn.Open();

                // NOTE: Ensure this stored procedure exists in your database:
                // CREATE PROCEDURE MyStoredProc @Param1 NVARCHAR(50) AS SELECT @Param1;
                using var cmd = new SqlCommand("MyStoredProc", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Param1", SqlDbType.NVarChar, 50).Value = "This is a test parameter.";

                Console.WriteLine("Executing stored procedure...");
                object? result = cmd.ExecuteScalar();

                Console.WriteLine(result != null
                    ? $"Stored procedure returned: '{result}'"
                    : "Stored procedure returned no value.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[SQL Error] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }

        static void SqlCommandBuilderDemo()
        {
            Console.WriteLine("--- SqlCommandBuilder Demo ---");
            Console.WriteLine("Automatically generates INSERT, UPDATE, and DELETE commands for a DataAdapter.");

            try
            {
                using var conn = new SqlConnection(connectionString);
                using var adapter = new SqlDataAdapter("SELECT Id, Name FROM YourTable", conn);
                using var builder = new SqlCommandBuilder(adapter);

                // Show the auto-generated UPDATE command
                var updateCommand = builder.GetUpdateCommand();
                Console.WriteLine("\nAuto-generated UPDATE Command:");
                Console.WriteLine(updateCommand?.CommandText ?? "? No UPDATE command generated.");

                var ds = new DataSet();
                int rowsFetched = adapter.Fill(ds, "YourTable");

                if (rowsFetched > 0)
                {
                    var table = ds.Tables["YourTable"];
                    if (table != null && table.Rows.Count > 0)
                    {
                        Console.WriteLine($"\nFetched {table.Rows.Count} row(s). Modifying the first row in memory...");
                        table.Rows[0]["Name"] = $"UpdatedName_{DateTime.Now:yyyyMMdd_HHmmssfff}";

                        Console.WriteLine("Pushing changes back to the database using adapter.Update()...");
                        int rowsUpdated = adapter.Update(ds, "YourTable");

                        Console.WriteLine(rowsUpdated > 0
                            ? $"Successfully updated {rowsUpdated} row(s) in the database."
                            : "No rows were updated.");
                    }
                }
                else
                {
                    Console.WriteLine("No data found. Ensure 'YourTable' exists and has rows.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[SQL Error] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }

        static void SqlBulkCopyDemo()
        {
            Console.WriteLine("--- SQL Bulk Copy ---");
            Console.WriteLine("High-performance bulk insertion of data from a DataTable to a SQL Server table.");

            var dt = new DataTable("BulkData");
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add("BulkName1");
            dt.Rows.Add("BulkName2");
            dt.Rows.Add("BulkName3");

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var bulkCopy = new SqlBulkCopy(conn)
            {
                DestinationTableName = "YourTable"
            };

            Console.WriteLine("Starting bulk copy of 3 rows...");
            bulkCopy.WriteToServer(dt);
            Console.WriteLine("Bulk copy completed successfully.");
        }

        static void TransactionsDemo()
        {
            Console.WriteLine("--- Database Transactions ---");
            Console.WriteLine("Ensuring multiple operations are treated as a single, atomic unit.");

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                Console.WriteLine("Attempting to insert a record within a transaction...");

                using (var cmd = new SqlCommand("INSERT INTO YourTable (Name) VALUES (@name)", conn, transaction))
                {
                    cmd.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = "TransactionTest";
                    cmd.ExecuteNonQuery();
                }

                // Example: Uncomment to force an error and trigger rollback
                // using (var cmd2 = new SqlCommand("INSERT INTO NonExistentTable (Name) VALUES ('fail')", conn, transaction))
                // {
                //     cmd2.ExecuteNonQuery();
                // }

                transaction.Commit();
                Console.WriteLine("Transaction committed successfully. Data saved.");
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
                Console.WriteLine($"[SQL Error] Transaction rolled back: {ex.Message}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"[Unexpected Error] Transaction rolled back: {ex.Message}");
            }
        }

        [SupportedOSPlatform("windows")]
        static void MsAccessConnection()
        {
            Console.WriteLine("--- Connecting to MS Access Database ---");
            using var conn = new OleDbConnection(accessConnectionString);
            conn.Open();

            Console.WriteLine("\n1. Inserting a new record using parameterized command.");
            using var insertCmd = new OleDbCommand("INSERT INTO YourAccessTable (Name, Age) VALUES (?, ?)", conn);
            // Note: OleDb uses '?' for parameters and they are position-based.
            insertCmd.Parameters.Add("?", OleDbType.VarWChar).Value = "New Access Record";
            insertCmd.Parameters.Add("?", OleDbType.Integer).Value = 30;
            int rows = insertCmd.ExecuteNonQuery();
            Console.WriteLine($"Inserted {rows} row(s).");

            Console.WriteLine("\n2. Reading data from 'YourAccessTable'.");
            using (var cmd = new OleDbCommand("SELECT TOP 5 ID, Name, Age FROM YourAccessTable", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader[0]}, Name: {reader[1]}, Age: {reader[2]}");
                }
            }
        }

        static void XmlDataReadWrite()
        {
            Console.WriteLine("--- XML Data Read/Write ---");

            try
            {
                // Create dataset and table
                var ds = new DataSet("Products");
                var dt = new DataTable("Product");
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                dt.Columns.Add("Price", typeof(decimal));

                // Sample data
                dt.Rows.Add(1, "Laptop", 1200.50m);
                dt.Rows.Add(2, "Mouse", 15.99m);
                ds.Tables.Add(dt);

                // Save XML file
                string xmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.xml");

                if (File.Exists(xmlFilePath))
                {
                    Console.WriteLine($"Overwriting existing file: {xmlFilePath}");
                }

                ds.WriteXml(xmlFilePath, XmlWriteMode.WriteSchema);
                Console.WriteLine($"XML file created at: {xmlFilePath}");

                // Read XML file into new DataSet
                var ds2 = new DataSet();
                ds2.ReadXml(xmlFilePath);

                Console.WriteLine("\nData read back from XML:");

                var table = ds2.Tables["Product"]; // compiler knows this may be null
                if (table?.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        Console.WriteLine($"ID: {row["Id"]}, Name: {row["Name"]}, Price: {row["Price"]}");
                    }
                }
                else
                {
                    Console.WriteLine("No 'Product' table found or table is empty.");
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"[File Error] {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }

        static void DisconnectedUpdateBackToSql()
        {
            Console.WriteLine("--- Disconnected Update Back to SQL Server ---");

            try
            {
                using var conn = new SqlConnection(connectionString);
                using var adapter = new SqlDataAdapter("SELECT Id, Name FROM YourTable", conn);
                using var builder = new SqlCommandBuilder(adapter);

                var ds = new DataSet();
                int rowsFetched = adapter.Fill(ds, "YourTable");

                if (rowsFetched > 0)
                {
                    var table = ds.Tables["YourTable"];
                    if (table != null)
                    {
                        Console.WriteLine($"Data loaded into DataSet ({table.Rows.Count} existing rows).");
                        Console.WriteLine("Adding a new row in memory...");

                        var newRow = table.NewRow();
                        newRow["Name"] = $"NewDisconnectedName_{DateTime.Now:yyyyMMdd_HHmmssfff}";
                        table.Rows.Add(newRow);

                        Console.WriteLine("Updating the SQL Server table from disconnected DataSet...");
                        int rowsUpdated = adapter.Update(ds, "YourTable");

                        Console.WriteLine(rowsUpdated > 0
                            ? $"{rowsUpdated} new row(s) successfully updated back to SQL Server."
                            : "No rows were updated.");
                    }
                    else
                    {
                        Console.WriteLine("'YourTable' exists in DB but could not be loaded into DataSet.");
                    }
                }
                else
                {
                    Console.WriteLine("No rows fetched from 'YourTable'. Disconnected update not performed.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"[SQL Error] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Unexpected Error] {ex.Message}");
            }
        }
    }
}