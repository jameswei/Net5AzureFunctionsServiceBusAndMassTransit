using System.Threading;
using System.Threading.Tasks;
using MassTransit.WebJobs.ServiceBusIntegration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LocalFunctionProj
{
    public class OnQueueMessage
    {
        const string QueueName = "testqueue";
        readonly IMessageReceiver _receiver;

        public OnQueueMessage(IMessageReceiver receiver) => _receiver = receiver;

        [Function("OnQueueMessage")]
        public Task OnTriggerAsync([ServiceBusTrigger(QueueName)] byte[] body,
            FunctionContext context)
        {
            var logger = context.GetLogger("OnQueueMessage");
            logger.LogDebug("Received queue msg");
            var message = MessageFactory.CreateMessage(body, context);

            return _receiver.Handle(QueueName, message, CancellationToken.None);
        }
    }
}