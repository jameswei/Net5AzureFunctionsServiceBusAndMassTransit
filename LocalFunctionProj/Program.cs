using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using MassTransit.Azure.ServiceBus.Core.Configuration;
using MassTransit.Azure.ServiceBus.Core.Configurators;
using MassTransit.WebJobs.ServiceBusIntegration;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.Services.AppAuthentication;
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

        static void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) =>
            {
                module.IncludeDiagnosticSourceActivities.Add("MassTransit");
            })
                .AddMassTransit(x =>
                {

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
            self.AddMassTransit(massTransit =>
            {
                massTransit.AddConsumers(typeof(Program).Assembly);
            });
        }

        static void WireUp(ContainerBuilder builder)
        {
            builder.RegisterType<Responder>().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                .Where(t => t.IsAssignableTo<IConsumer>() && !t.IsAbstract)
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.AddMassTransitForFunctions();
        }
    }
}