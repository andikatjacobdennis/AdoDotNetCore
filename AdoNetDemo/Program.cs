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

        // Using a static readonly field for JsonSerializerOptions to avoid creating it multiple times
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        static void Main()
        {
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
                Console.WriteLine("12. Parameterized Commands");
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
                    Console.WriteLine($"[ERROR] {ex.Message}");
                }

                Console.WriteLine("\nPress Enter to return to menu...");
                Console.ReadLine();
            }
        }

        static void Introduction()
        {
            Console.WriteLine("ADO.NET is a set of classes for accessing data sources from .NET.");
            Console.WriteLine("Supports Connected & Disconnected models using SqlConnection, SqlCommand, DataSet, etc.");
        }

        static void EnvironmentSetup()
        {
            Console.WriteLine("Steps:");
            Console.WriteLine("1. Install Visual Studio 2022 or Visual Studio Code:");
            Console.WriteLine("   Visual Studio 2022: https://visualstudio.microsoft.com/vs/");
            Console.WriteLine("   Visual Studio Code: https://code.visualstudio.com/");
            Console.WriteLine();
            Console.WriteLine("2. Install .NET 9 SDK:");
            Console.WriteLine("   https://dotnet.microsoft.com/en-us/download/dotnet/9.0");
            Console.WriteLine();
            Console.WriteLine("3. Create Console Project:");
            Console.WriteLine("   dotnet new console -n AdoNetDemo -f net9.0");
            Console.WriteLine();
            Console.WriteLine("4. Install Microsoft.Data.SqlClient NuGet package (version 6.0.2):");
            Console.WriteLine("   https://www.nuget.org/packages/Microsoft.Data.SqlClient/6.0.2");
            Console.WriteLine();
            Console.WriteLine("5. Install System.Data.OleDb NuGet package (version 9.0.8):");
            Console.WriteLine("   https://www.nuget.org/packages/System.Data.OleDb/9.0.8");
        }

        static void DisconnectedArchitecture()
        {
            using var conn = new SqlConnection(connectionString);
            var adapter = new SqlDataAdapter("SELECT TOP 5 Id, Name FROM YourTable", conn);
            var dataSet = new DataSet();
            adapter.Fill(dataSet, "YourTable");

            var table = dataSet.Tables["YourTable"];
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine($"{row["Id"]} - {row["Name"]}");
                }
            }
            else
            {
                Console.WriteLine("Table 'YourTable' not found in DataSet.");
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

            Console.WriteLine($"DataSet: {ds.DataSetName}, Tables: {ds.Tables.Count}");
        }

        static void SerializingDataSet()
        {
            var ds = new DataSet("PeopleDS");
            var dt = new DataTable("People");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");
            ds.Tables.Add(dt);

            // Write full DataSet to XML
            ds.WriteXml("people.xml");

            // Flatten DataTable to a simple list to avoid cycles in JSON
            var peopleList = new List<object>();
            var table = ds.Tables["People"];
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    peopleList.Add(new
                    {
                        Id = row["Id"],
                        Name = row["Name"]
                    });
                }
            }

            // Serialize to JSON
            string json = JsonSerializer.Serialize(peopleList, _jsonOptions);
            File.WriteAllText("people.json", json);

            Console.WriteLine("Serialized to XML & JSON (clean JSON array).");
            Console.WriteLine("\nJSON Output:");
            Console.WriteLine(json);
        }

        static void ConnectedArchitecture()
        {
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
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            Console.WriteLine("Connection established successfully!");
        }

        static void ExecutingCommands()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT COUNT(*) FROM YourTable", conn);
            int count = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Total Rows: {count}");
        }

        static void CrudOperations()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();

            // CREATE (parameterized)
            var insertCmd = new SqlCommand("INSERT INTO YourTable (Name) VALUES (@name)", conn);
            insertCmd.Parameters.AddWithValue("@name", "SecureName");
            insertCmd.ExecuteNonQuery();
            Console.WriteLine("Inserted record.");

            // READ
            var selectCmd = new SqlCommand("SELECT TOP 1 Id, Name FROM YourTable ORDER BY Id DESC", conn);
            using var reader = selectCmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}");
            }
        }

        static void ReadingBulkData()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM LargeTable", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader[0]);
            }
        }

        static void SqlInjectionExample()
        {
            Console.WriteLine("This is an example of a vulnerable approach (DO NOT USE):");
            Console.WriteLine("SELECT * FROM YourTable WHERE Id = " + "user input");
            Console.WriteLine("Instead, always use parameters to prevent SQL Injection.");
        }

        static void ParameterizedCommands()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = new SqlCommand("SELECT * FROM YourTable WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", 1);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine(reader["Name"]);
            }
        }

        static void ExecutingProcedures()
        {
            Console.WriteLine("Starting procedure execution...");

            using var conn = new SqlConnection(connectionString);
            Console.WriteLine("Opening SQL connection...");
            conn.Open();
            Console.WriteLine("Connection opened.");

            Console.WriteLine("Preparing stored procedure command: MyStoredProc");
            var cmd = new SqlCommand("MyStoredProc", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            Console.WriteLine("Adding parameter: @Param1 = 'Value'");
            cmd.Parameters.AddWithValue("@Param1", "Value");

            Console.WriteLine("Executing stored procedure...");
            int rowsAffected = cmd.ExecuteNonQuery();
            Console.WriteLine($"Stored procedure executed successfully. Rows affected: {rowsAffected}");

            Console.WriteLine("Procedure execution completed.");
        }

        static void SqlCommandBuilderDemo()
        {
            using var conn = new SqlConnection(connectionString);

            // Load some sample rows
            var adapter = new SqlDataAdapter("SELECT Id, Name FROM YourTable", conn);

            // SqlCommandBuilder auto-generates Insert/Update/Delete based on SelectCommand
            var builder = new SqlCommandBuilder(adapter);

            // Show generated commands
            Console.WriteLine("=== Commands auto-generated by SqlCommandBuilder ===");
            Console.WriteLine("\nINSERT Command:\n" + builder.GetInsertCommand().CommandText);
            Console.WriteLine("\nUPDATE Command:\n" + builder.GetUpdateCommand().CommandText);
            Console.WriteLine("\nDELETE Command:\n" + builder.GetDeleteCommand().CommandText);

            // Fill DataSet
            var ds = new DataSet();
            adapter.Fill(ds, "YourTable");
            var table = ds.Tables["YourTable"];

            if (table != null && table.Rows.Count > 0)
            {
                // Modify the first row
                Console.WriteLine("\nModifying first row in memory...");
                table.Rows[0]["Name"] = "UpdatedName_" + DateTime.Now.Ticks;

                // Push changes back to DB
                int rowsUpdated = adapter.Update(ds, "YourTable");
                Console.WriteLine($"Updated {rowsUpdated} row(s) in the database.");
            }
            else
            {
                Console.WriteLine("\nNo data found to update.");
            }
        }

        static void SqlBulkCopyDemo()
        {
            Console.WriteLine("Starting bulk copy...");

            var dt = new DataTable();
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add("Name1");
            dt.Rows.Add("Name2");

            using var conn = new SqlConnection(connectionString);
            conn.Open();

            using var bulkCopy = new SqlBulkCopy(conn)
            {
                DestinationTableName = "YourTable"
            };
            bulkCopy.WriteToServer(dt);

            Console.WriteLine("Bulk copy completed.");
        }

        static void TransactionsDemo()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var transaction = conn.BeginTransaction();

            try
            {
                var cmd = new SqlCommand("INSERT INTO YourTable (Name) VALUES (@name)", conn, transaction);
                cmd.Parameters.AddWithValue("@name", "TransactionTest");
                cmd.ExecuteNonQuery();
                transaction.Commit();
                Console.WriteLine("Transaction committed.");
            }
            catch
            {
                transaction.Rollback();
                Console.WriteLine("Transaction rolled back.");
            }
        }

        [SupportedOSPlatform("windows")]
        static void MsAccessConnection()
        {
            using var conn = new OleDbConnection(accessConnectionString);
            conn.Open();

            // First read: fetch first 5 rows
            Console.WriteLine("=== First Read ===");
            using (var cmd = new OleDbCommand("SELECT TOP 5 * FROM YourAccessTable", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]} - {reader[1]}");
                }
            }

            // Write: insert a test row
            Console.WriteLine("\n=== Writing a new record ===");
            using (var insertCmd = new OleDbCommand(
                "INSERT INTO YourAccessTable (Name, Age) VALUES (@val1, @val2)", conn))
            {
                insertCmd.Parameters.AddWithValue("@val1", "Name");
                insertCmd.Parameters.AddWithValue("@val2", 20);
                int rows = insertCmd.ExecuteNonQuery();
                Console.WriteLine($"Inserted {rows} row(s).");
            }

            // Second read: fetch last 5 rows (to see the new record)
            Console.WriteLine("\n=== Second Read ===");
            using var cmd2 = new OleDbCommand("SELECT TOP 5 * FROM YourAccessTable ORDER BY ID DESC", conn);
            using var reader2 = cmd2.ExecuteReader();
            while (reader2.Read())
            {
                Console.WriteLine($"{reader2[0]} - {reader2[1]}");
            }
        }
        static void XmlDataReadWrite()
        {
            var ds = new DataSet("Products");
            var dt = new DataTable("Product");
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Price", typeof(decimal));

            dt.Rows.Add(1, "Laptop", 1200.50m);
            dt.Rows.Add(2, "Mouse", 15.99m);

            ds.Tables.Add(dt);

            // Write to XML
            ds.WriteXml("products.xml");
            Console.WriteLine("XML file 'products.xml' created.");

            // Read back from XML
            var ds2 = new DataSet();
            ds2.ReadXml("products.xml");

            Console.WriteLine("\nData read from XML:");
            var table = ds2.Tables["Product"];
            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine($"{row["Id"]} - {row["Name"]} - {row["Price"]}");
                }
            }
            else
            {
                Console.WriteLine("Table 'Product' not found in XML.");
            }
        }

        static void DisconnectedUpdateBackToSql()
        {
            using var conn = new SqlConnection(connectionString);
            var adapter = new SqlDataAdapter("SELECT Id, Name FROM YourTable", conn);
            var builder = new SqlCommandBuilder(adapter);

            var ds = new DataSet();
            adapter.Fill(ds, "YourTable");

            var table = ds.Tables["YourTable"];
            if (table != null)
            {
                Console.WriteLine("Current Data:");
                foreach (DataRow row in table.Rows)
                {
                    Console.WriteLine($"{row["Id"]} - {row["Name"]}");
                }

                var newRow = table.NewRow();
                newRow["Name"] = "NewDisconnectedName";
                table.Rows.Add(newRow);

                int rowsUpdated = adapter.Update(ds, "YourTable");
                Console.WriteLine($"{rowsUpdated} row(s) updated back to SQL Server.");
            }
            else
            {
                Console.WriteLine("Table 'YourTable' not found.");
            }
        }
    }
}