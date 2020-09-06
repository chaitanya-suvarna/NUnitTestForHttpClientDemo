using System;
using System.Threading.Tasks;

namespace EmployeeApiClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            EmployeeApiClientService employeeApiClientService = new EmployeeApiClientService(new System.Net.Http.HttpClient());

            Employee employeeEntity = await employeeApiClientService.GetEmployeeAsync(1);

            Console.WriteLine("Employee Details :");
            Console.WriteLine($"Name : {employeeEntity.data.employee_name}");
            Console.WriteLine($"Age : {employeeEntity.data.employee_age}");
            Console.WriteLine($"Salary : {employeeEntity.data.employee_salary}");

            Console.ReadLine();
        }
    }
}
