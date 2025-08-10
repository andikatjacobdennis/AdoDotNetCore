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

        public void GetEmployeesUsingStoredProcedure()
        {
            string procedureName = "sp_GetEmployees";
            using var reader = _dbCommandExecutor.ExecuteStoredProcedure(procedureName);

            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
            }
        }

        public DataSet GetEmployeesDataSet()
        {
            string query = "SELECT * FROM Employees";
            var dataSet = new DataSet();

            using var connection = _dbCommandExecutor._dbConnectionManager.GetConnection();
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

        public async Task GetEmployeesAsync()
        {
            string query = "SELECT * FROM Employees";
            using var reader = await _dbCommandExecutor.ExecuteReaderAsync(query);

            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Age: {reader["Age"]}");
            }
        }
    }
}