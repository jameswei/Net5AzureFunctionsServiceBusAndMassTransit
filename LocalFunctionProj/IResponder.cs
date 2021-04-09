using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;

namespace LocalFunctionProj
{
    public interface IResponder
    {
        Task<HttpResponseData> RespondToAsync(HttpRequestData request);
    }
}