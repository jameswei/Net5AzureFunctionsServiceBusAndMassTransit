using System;
using System.Threading.Tasks;
using MassTransit;
using SharedContracts;

namespace TestClient
{
    public class SendRequestCommand : AQueueUser, ICommand
    {
        public SendRequestCommand(IBus bus) : base(bus) { }

        public Task ExecuteAsync() =>
            DoWithBusAsync(async (bus, queueAddress) =>
            {
                var requestClient = bus.CreateRequestClient<NumberRequest>(queueAddress);
                var response = await requestClient.GetResponse<NumberResponse>(Request);
                var responseMessage = response.Message;
                Console.WriteLine($"Received answer: {responseMessage.Result}");
            });

        public NumberRequest Request { get; } = new();
    }
}