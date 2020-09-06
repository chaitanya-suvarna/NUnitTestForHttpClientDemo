using EmployeeApiClient;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NUnitTestHttpClient
{
    public class HttpMessageHandlerStub : HttpMessageHandler
    {
        public HttpRequestMessage httpRequestMessage;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            httpRequestMessage = request;
            return Task.Run(() => new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(new Employee()), Encoding.UTF8, "application/json")
            });
        }
    }
}
