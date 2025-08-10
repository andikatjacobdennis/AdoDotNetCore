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