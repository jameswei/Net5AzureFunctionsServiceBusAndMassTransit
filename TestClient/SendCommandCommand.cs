using System;
using System.Threading.Tasks;
using MassTransit;
using SharedContracts;

namespace TestClient
{
    public class SendCommandCommand : AQueueUser, ICommand
    {
        public SendCommandCommand(IBus bus) : base(bus) { }

        public Task ExecuteAsync() =>
            DoWithBusAsync(async (bus, queueAddress) =>
            {
                var endpoint = await bus.GetSendEndpoint(queueAddress);
                await endpoint.Send(Cmd);
                Console.WriteLine("...command sent");
            });

        public SomeCommand Cmd { get; } = new();
    }
}