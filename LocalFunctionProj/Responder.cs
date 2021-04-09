using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocalFunctionProj
{
    public class Responder : IResponder
    {
        public async Task<HttpResponseData> RespondToAsync(HttpRequestData request)
        {
            await Task.Delay(200);
            var response = request.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            await response.WriteStringAsync("Welcome to async Azure Functions!");
            return response;
        }
    }
}