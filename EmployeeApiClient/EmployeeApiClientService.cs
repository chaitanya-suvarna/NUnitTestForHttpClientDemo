using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace EmployeeApiClient
{
    public class EmployeeApiClientService
    {
        private readonly HttpClient employeeHttpClient;

        public EmployeeApiClientService(HttpClient httpClient)
        {
            employeeHttpClient = httpClient;
        }

        //environment specific variables should always be set in a seperate config file or database. 
        //For the sake of this example I'm initialising them here.
        public static string testDatabase = "SloughDB";
        public static string environment = "TEST";

        public async Task<Employee> GetEmployeeAsync(int employeeId)
        {
            Employee employee = null;

            //Add headers
            employeeHttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            employeeHttpClient.DefaultRequestHeaders.Add("tracking-id", Guid.NewGuid().ToString());

            //Conditional Headers
            if (environment == "TEST")
            {
                employeeHttpClient.DefaultRequestHeaders.Add("test-db", testDatabase);
            }

            HttpResponseMessage response = await employeeHttpClient.GetAsync("http://dummy.restapiexample.com/api/v1/employee/1");
            if (response.IsSuccessStatusCode)
            {
                employee = JsonConvert.DeserializeObject<Employee>(await response.Content.ReadAsStringAsync());
            }
            return employee;
        }
    }

    public class Employee
    {
        public string status { get; set; }
        public Data data { get; set; }
        public string message { get; set; }
    }

    public class Data
    {
        public int id { get; set; }
        public string employee_name { get; set; }
        public int employee_salary { get; set; }
        public int employee_age { get; set; }
        public string profile_image { get; set; }
    }

}
