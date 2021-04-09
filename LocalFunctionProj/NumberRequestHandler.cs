using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts;

namespace LocalFunctionProj
{
    public class NumberRequestHandler : IConsumer<NumberRequest>
    {
        readonly ILogger<NumberRequestHandler> _logger;
        public NumberRequestHandler(ILogger<NumberRequestHandler> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<NumberRequest> context)
        {
            var request = context.Message;
            var logMsg = $"Received NumberRequest with {request.Number}";
            _logger.LogDebug(logMsg);
            Console.WriteLine(logMsg);
            var response = new NumberResponse() {Result = request.Number * 2};
            logMsg = $"Sending response with {response.Result}";
            _logger.LogDebug(logMsg);
            Console.WriteLine(logMsg);
            await context.RespondAsync(response);
        }
    }
}