using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.WebJobs.ServiceBusIntegration;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LocalFunctionProj
{
    public static class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration(c => c.AddEnvironmentVariables())
                .UseServiceProviderFactory(new AutofacServiceProviderFactory(WireUp))
                .ConfigureServices(ConfigureServices)
                .Build();

            host.Run();
        }

        // 在 ServiceCollection 上注册 service
        static void ConfigureServices(IServiceCollection services)
        {
            // telemetry module
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
                module.IncludeDiagnosticSourceActivities.Add("MassTransit");
            })
                // 配置 MassTransit
                .AddMassTransit(x =>
                {
                    // 使用 ASB 作为 transport provider
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        var config = context.GetRequiredService<IConfiguration>();
                        var connectionString = config["AzureWebJobsServiceBus"];
                        var options = context.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
                        options.ConnectionString = connectionString;
                        options.MessageHandlerOptions.AutoComplete = true;
                        options.MessageHandlerOptions.MaxAutoRenewDuration = TimeSpan.FromMinutes(30);
                        options.MessageHandlerOptions.MaxConcurrentCalls = 32;
                        options.BatchOptions.OperationTimeout = TimeSpan.FromMinutes(1);
                        options.PrefetchCount = 32;

                        cfg.Host(options.ConnectionString);
                        cfg.UseServiceBusMessageScheduler();

                    });
                });

        }
        public static void AddMassTransitForFunctions(this ContainerBuilder self)
        {
            self.RegisterType<AsyncBusHandle>().As<IAsyncBusHandle>().SingleInstance();
            self.RegisterType<MessageReceiver>().As<IMessageReceiver>().SingleInstance();
            // 向 autofac 注册 MassTransit
            self.AddMassTransit(massTransit =>
            {
                massTransit.AddConsumers(typeof(Program).Assembly);
            });
        }

        // 利用Autofac装配service provider
        static void WireUp(ContainerBuilder builder)
        {
            builder.RegisterType<Responder>().AsImplementedInterfaces();
            // 注册IConsumer实现类
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => t.IsAssignableTo<IConsumer>() && !t.IsAbstract)
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.AddMassTransitForFunctions();
        }
    }
}