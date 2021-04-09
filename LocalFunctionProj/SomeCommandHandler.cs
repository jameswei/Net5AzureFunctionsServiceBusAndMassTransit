using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts;

namespace LocalFunctionProj
{
    public class SomeCommandHandler : IConsumer<SomeCommand>
    {
        readonly ILogger<SomeCommandHandler> _logger;
        public SomeCommandHandler(ILogger<SomeCommandHandler> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SomeCommand> context)
        {
            var cmd = context.Message;
            var logMsg = $"Received SomeCommand with {cmd.Text}";
            _logger.LogDebug(logMsg);
            Console.WriteLine(logMsg);
            return Task.CompletedTask;
        }
    }
}