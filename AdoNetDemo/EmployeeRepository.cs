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