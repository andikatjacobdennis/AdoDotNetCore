namespace AdoNetDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Data Source=.;Initial Catalog=AdoNetDemo;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

            var dbConnectionManager = new DbConnectionManager(connectionString);
            var dbCommandExecutor = new DbCommandExecutor(dbConnectionManager);
            var employeeRepository = new EmployeeRepository(dbCommandExecutor);

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

            Console.WriteLine("\n--- Program complete ---");
        }
    }
}