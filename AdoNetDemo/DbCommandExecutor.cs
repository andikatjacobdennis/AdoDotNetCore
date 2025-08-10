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