using System.Threading;
using System.Threading.Tasks;
using MassTransit.WebJobs.ServiceBusIntegration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace LocalFunctionProj
{
    public class OnBusNotification
    {
        const string QueueName = "sharedcontracts~somenotification";
        readonly IMessageReceiver _receiver;

        public OnBusNotification(IMessageReceiver receiver) => _receiver = receiver;

        [Function("OnBusNotification")]
        public Task OnTriggerAsync([ServiceBusTrigger(QueueName, "my-subscription")] byte[] rawMessage,
            FunctionContext context)
        {
            var logger = context.GetLogger("OnBusNotification");
            logger.LogDebug("Received topic notification msg");
            var message = new Message(rawMessage);
            return _receiver.Handle(QueueName, message, CancellationToken.None);
        }
    }
}