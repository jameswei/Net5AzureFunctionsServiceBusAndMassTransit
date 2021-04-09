using System;
using System.IO;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestClient
{
    public static class ConsoleApplication
    {
        public static Task BootstrapAsync<TModule, TApplication>(string applicationName, string[] args)
            where TModule : AModuleWithConfiguration, new() where TApplication : class, IHostedService
        {
            IConfiguration configuration = default;
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    configuration = context.Configuration;
                    services.AddHostedService<TApplication>();
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(b =>
                    b.RegisterModule(new TModule {Configuration = configuration}));
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                $"{applicationName}.json");
            builder.ConfigureAppConfiguration((_, cfg) => cfg.AddJsonFile(path, optional: true));
            return builder
                .Build()
                .RunAsync();
        }
    }
}