using EmployeeApiClient;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTestHttpClient
{
    [TestFixture]
    class EmployeeApiClientServiceTests
    {
        EmployeeApiClientService employeeApiClientService;
        Mock<HttpMessageHandler> httpMessageHandlerMock;

        //environment specific variables should always be set in a separate config file or database. 
        //For the sake of this example I'm initialising them here.
        string testDatabase = "SloughDB";
        string environment = "TEST";

        [SetUp]
        public void setUp()
        {
            httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            HttpClient httpClient = new HttpClient(httpMessageHandlerMock.Object);
            employeeApiClientService = new EmployeeApiClientService(httpClient);
        }


        [Test]
        public async Task GivenICallGetEmployeeAsyncWithValidEmployeeId_ThenTheEmployeeApiIsCalledWithCorrectRequestHeadersAsync()
        {
            //Arrange
            int employeeId = 1;
            string targetUri = $"http://dummy.restapiexample.com/api/v1/employee/{employeeId}";
            //Setup sendAsync method for HttpMessage Handler Mock
            httpMessageHandlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new Employee()), Encoding.UTF8, "application/json")
                })
                .Verifiable();

            //Act
            var employee = await employeeApiClientService.GetEmployeeAsync(employeeId);

            //Assert
            Assert.IsInstanceOf<Employee>(employee);
            
            httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(1), // verify number of times SendAsync is called
                ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Get  // verify the HttpMethod for request is GET
                && req.RequestUri.ToString() == targetUri // veryfy the RequestUri is as expected
                && req.Headers.GetValues("Accept").FirstOrDefault() == "application/json" //Verify Accept header
                && req.Headers.GetValues("tracking-id").FirstOrDefault() != null //Verify tracking-id header is added
                && environment.Equals("TEST") ? req.Headers.GetValues("test-db").FirstOrDefault() == testDatabase : //Verify test-db header is added only for TEST environment
                                                req.Headers.GetValues("test-db").FirstOrDefault() == null 
                ),
                ItExpr.IsAny<CancellationToken>()
                );
        }
    }
}
