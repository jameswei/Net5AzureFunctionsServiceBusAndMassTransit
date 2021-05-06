using System;
using Autofac;
using MassTransit;

namespace TestClient
{
    // 定义 autofac 的 module
    public class TestClientModule : AModuleWithConfiguration
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Application>().AsSelf();
            builder.RegisterType<CommandFactory>().AsImplementedInterfaces();
            builder.RegisterType<SendCommandCommand>().AsSelf();
            builder.RegisterType<SendNotificationCommand>().AsSelf();
            builder.RegisterType<SendRequestCommand>().AsSelf();
            // 向 autofac 注册 MassTransit
            builder.AddMassTransit(massTransit =>
            {
                // 使用ASB 作为 transport provider
                massTransit.AddBus(ctx => Bus.Factory.CreateUsingAzureServiceBus(cfg =>
                {
                    var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
                    cfg.Host(connectionString);
                }));
            });
        }
    }
}