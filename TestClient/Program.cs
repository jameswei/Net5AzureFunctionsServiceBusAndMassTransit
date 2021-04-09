using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static Task Main(string[] args) => ConsoleApplication.BootstrapAsync<TestClientModule, Application>("TestClient", args);
    }
}
