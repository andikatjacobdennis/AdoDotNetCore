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

        public void ExecuteNonQuery(string query)
        {
            using SqlConnection connection = _dbConnectionManager.GetConnection();
            connection.Open();

            using SqlCommand command = new SqlCommand(query, connection);
            command.ExecuteNonQuery();
        }

        public SqlDataReader ExecuteReader(string query)
        {
            SqlConnection connection = _dbConnectionManager.GetConnection();
            connection.Open();

            SqlCommand command = new SqlCommand(query, connection);
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public object ExecuteScalar(string query)
        {
            using SqlConnection connection = _dbConnectionManager.GetConnection();
            connection.Open();

            using SqlCommand command = new SqlCommand(query, connection);
            return command.ExecuteScalar();
        }

        public SqlDataReader ExecuteStoredProcedure(string procedureName, SqlParameter[]? parameters = null)
        {
            SqlConnection connection = _dbConnectionManager.GetConnection();
            connection.Open();

            SqlCommand command = new SqlCommand(procedureName, connection);
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public async Task<SqlDataReader> ExecuteReaderAsync(string query)
        {
            SqlConnection connection = _dbConnectionManager.GetConnection();
            await connection.OpenAsync();

            SqlCommand command = new SqlCommand(query, connection);
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
        }

        public async Task ExecuteNonQueryAsync(string query)
        {
            using SqlConnection connection = _dbConnectionManager.GetConnection();
            await connection.OpenAsync();

            using SqlCommand command = new SqlCommand(query, connection);
            await command.ExecuteNonQueryAsync();
        }
    }
}