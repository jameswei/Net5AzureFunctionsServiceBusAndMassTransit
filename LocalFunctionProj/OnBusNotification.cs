using System.Threading;
using System.Threading.Tasks;
using MassTransit.WebJobs.ServiceBusIntegration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace LocalFunctionProj
{
    // POCO 方式实现 azure function
    public class OnBusNotification
    {
        const string QueueName = "sharedcontracts~somenotification";
        readonly IMessageReceiver _receiver;

        public OnBusNotification(IMessageReceiver receiver) => _receiver = receiver;

        // ServiceBusTrigger 可以加在byte[]参数上
        [Function("OnBusNotification")]
        public Task OnTriggerAsync([ServiceBusTrigger(QueueName, "my-subscription")] byte[] rawMessage,
            FunctionContext context)
        {
            var logger = context.GetLogger("OnBusNotification");
            logger.LogDebug("Received topic notification msg");
            // 直接从 byte[]来组装 message
            var message = new Message(rawMessage);
            return _receiver.Handle(QueueName, message, CancellationToken.None);
        }
    }
}