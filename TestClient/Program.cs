using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            var task = ConsoleApplication.BootstrapAsync<TestClientModule, Application>("TestClient", args);
            task.Start();
        }
        // static Task Main(string[] args) => ConsoleApplication.BootstrapAsync<TestClientModule, Application>("TestClient", args);
    }
}
