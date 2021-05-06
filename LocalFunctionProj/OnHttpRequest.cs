using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace LocalFunctionProj
{
    public class OnHttpRequest
    {
        readonly IResponder _responder;

        public OnHttpRequest(IResponder responder) => _responder = responder;

        // HttpTrigger 加在 HttpRequestData 参数上，由Worker来组装参数
        [Function("OnHttpRequest")]
        public Task<HttpResponseData> OnTriggerAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
            HttpRequestData request,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("OnHttpRequest");
            logger.LogInformation("C# HTTP trigger function processed a request.");

            return _responder.RespondToAsync(request);
        }
    }
}