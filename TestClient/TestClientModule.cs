using System;
using Autofac;
using MassTransit;

namespace TestClient
{
    public class TestClientModule : AModuleWithConfiguration
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Application>().AsSelf();
            builder.RegisterType<CommandFactory>().AsImplementedInterfaces();
            builder.RegisterType<SendCommandCommand>().AsSelf();
            builder.RegisterType<SendNotificationCommand>().AsSelf();
            builder.RegisterType<SendRequestCommand>().AsSelf();

            builder.AddMassTransit(massTransit =>
            {
                massTransit.AddBus(ctx => Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                {
                    var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
                    cfg.Host(connectionString);
                }));
            });
        }
    }
}