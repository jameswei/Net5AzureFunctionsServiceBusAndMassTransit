using Autofac;
using Microsoft.Extensions.Configuration;

namespace TestClient
{
    public abstract class AModuleWithConfiguration : Module
    {
        public IConfiguration Configuration { get; set; }
    }
}