using System;
using System.Threading.Tasks;
using MassTransit;
using SharedContracts;

namespace TestClient
{
    public class SendNotificationCommand : AQueueUser, ICommand
    {
        public SendNotificationCommand(IBus bus) : base(bus) { }

        public Task ExecuteAsync() =>
            DoWithBusAsync(async (bus, _) =>
            {
                await bus.Publish(Notification);
                Console.WriteLine("...notification sent");
            });

        public SomeNotification Notification { get; } = new();
    }
}