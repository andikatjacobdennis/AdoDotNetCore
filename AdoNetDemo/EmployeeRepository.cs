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

        // Check if database exists using a parameterized query
        public bool DatabaseExists(string databaseName)
        {
            // Build connection string with the same server & credentials but database = master
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_dbCommandExecutor._dbConnectionManager.GetConnection().ConnectionString)
            {
                InitialCatalog = "master"  // Override database to master
            };

            string query = "SELECT database_id FROM sys.databases WHERE Name = @dbName";
            SqlParameter[] parameters = {
                new SqlParameter("@dbName", SqlDbType.NVarChar) { Value = databaseName }
            };

            // Use the ExecuteScalar method to securely check for database existence
            using SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();
            using SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddRange(parameters);

            object result = command.ExecuteScalar();
            return result != null && result != DBNull.Value;
        }

        // Create method now uses a parameterized query to prevent SQL injection
        public void CreateEmployee(string name, int age)
        {
            string query = "INSERT INTO Employees (Name, Age) VALUES (@name, @age)";
            SqlParameter[] parameters = {
                new SqlParameter("@name", SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@age", SqlDbType.Int) { Value = age }
            };
            _dbCommandExecutor.ExecuteNonQuery(query, parameters);
        }

        // Read
        public void GetEmployees()
        {
            string query = "SELECT * FROM Employees";
            using SqlDataReader reader = _dbCommandExecutor.ExecuteReader(query);

            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
            }
        }

        // Update method uses a parameterized query to prevent SQL injection
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

        // Delete method uses a parameterized query to prevent SQL injection
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
            using SqlDataReader reader = _dbCommandExecutor.ExecuteStoredProcedure(procedureName);

            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
            }
        }

        public DataSet GetEmployeesDataSet()
        {
            string query = "SELECT * FROM Employees";
            DataSet dataSet = new DataSet();

            using SqlConnection connection = _dbCommandExecutor._dbConnectionManager.GetConnection();
            connection.Open();

            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            adapter.Fill(dataSet);

            return dataSet;
        }

        public void GetEmployeesUsingDataSet()
        {
            DataSet dataSet = GetEmployeesDataSet();

            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Console.WriteLine($"Id: {row["Id"]}, Name: {row["Name"]}, Age: {row["Age"]}");
            }
        }

        public async Task GetEmployeesAsync()
        {
            string query = "SELECT * FROM Employees";
            using SqlDataReader reader = await _dbCommandExecutor.ExecuteReaderAsync(query);

            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
            }
        }
    }
}