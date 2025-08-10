namespace AdoNetDemo
{
    class Program
    {
        static async Task Main()
        {
            string serverName = ".";
            string databaseName = "AdoNetDemoDb";
            string connectionString = $"Data Source={serverName};Initial Catalog={databaseName};Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            DbConnectionManager dbConnectionManager = new DbConnectionManager(connectionString);
            DbCommandExecutor dbCommandExecutor = new DbCommandExecutor(dbConnectionManager);
            EmployeeRepository employeeRepository = new EmployeeRepository(dbCommandExecutor);

            if (employeeRepository.DatabaseExists(databaseName))
            {
                Console.WriteLine("Database exists.");

                Console.WriteLine("--- Reading all employees ---");
                employeeRepository.GetEmployees();

                Console.WriteLine("\n--- Creating a new employee ---");
                employeeRepository.CreateEmployee("New Employee", 35);
                employeeRepository.GetEmployees();

                Console.WriteLine("\n--- Updating an employee (Id: 3) ---");
                employeeRepository.UpdateEmployee(3, "Updated Employee", 40);
                employeeRepository.GetEmployees();

                Console.WriteLine("\n--- Deleting an employee (Id: 4) ---");
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
                Console.WriteLine("Database does not exist.");
            }

            Console.WriteLine("\n--- Program complete ---");
        }
    }
}