using EmployeeApiClient;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NUnitTestHttpClient
{
    [TestFixture]
    class EmployeeApiClientServiceTests
    {
        EmployeeApiClientService employeeApiClientService;
        HttpMessageHandlerStub handlerStub;
        HttpClient httpClient;

        //environment specific variables should always be set in a seperate config file or database. 
        //For the sake of this example I'm initialising them here.
        string testDatabase = "SloughDB";
        string environment = "TEST";

        [SetUp]
        public void setUp()
        {
            handlerStub = new HttpMessageHandlerStub();
            httpClient = new HttpClient(handlerStub);
            employeeApiClientService = new EmployeeApiClientService(httpClient);
        }


        [Test]
        public async Task GivenICallGetEmployeeAsyncWithValidEmployeeId_ThenTheEmployeeApiIsCalledWithCorrectRequestHeadersAsync()
        {
            //Arrange
            int employeeId = 1;
            string route = $"http://dummy.restapiexample.com/api/v1/employee/{employeeId}";

            //Act
            var employee = await employeeApiClientService.GetEmployeeAsync(employeeId);

            //Assert
            Assert.IsInstanceOf<Employee>(employee);
            Assert.AreEqual(handlerStub.httpRequestMessage.RequestUri, route);
            Assert.AreEqual(handlerStub.httpRequestMessage.Headers.GetValues("Accept").FirstOrDefault(), "application/json");
            Assert.IsNotNull(handlerStub.httpRequestMessage.Headers.GetValues("tracking-id").FirstOrDefault());

            //conditional check for test-db header
            if(environment.Equals("TEST"))
            {
                Assert.AreEqual(handlerStub.httpRequestMessage.Headers.GetValues("test-db").FirstOrDefault(),testDatabase);
            }
            else
            {
                Assert.IsNull(handlerStub.httpRequestMessage.Headers.GetValues("test-db").FirstOrDefault());
            }
            
        }
    }
}
