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