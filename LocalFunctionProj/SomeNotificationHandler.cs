using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedContracts;

namespace LocalFunctionProj
{
    public class SomeNotificationHandler : IConsumer<SomeNotification>
    {
        readonly ILogger<SomeNotificationHandler> _logger;
        public SomeNotificationHandler(ILogger<SomeNotificationHandler> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<SomeNotification> context)
        {
            var notification = context.Message;
            var logMsg = $"Receveid SomeNotification with {notification.Info}";
            _logger.LogDebug(logMsg);
            Console.WriteLine(logMsg);
            return Task.CompletedTask;
        }
    }
}