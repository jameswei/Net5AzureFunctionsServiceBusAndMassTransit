using System;
using System.Threading.Tasks;
using MassTransit;

namespace TestClient
{
    public abstract class AQueueUser
    {
        const string QueueName = "testqueue";
        readonly IBus _bus;
        protected AQueueUser(IBus bus) => _bus = bus;

        protected async Task DoWithBusAsync(Func<IBus, Uri, Task> action)
        {
            var queueAddress = new Uri($"{_bus.Address.Scheme}://{_bus.Address.Host}/{QueueName}");
            try
            {
                await action(_bus, queueAddress);
            }
            catch (RequestTimeoutException)
            {
                await Console.Error.WriteLineAsync("The request has timed out.");
            }
            catch (RequestFaultException)
            {
                await Console.Error.WriteLineAsync("The request faulted on the other end");
            }
        }
    }
}