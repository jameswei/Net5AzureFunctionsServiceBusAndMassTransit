using System.Threading.Tasks;

namespace TestClient
{
    public class ExitCommand : ICommand
    {
        public Task ExecuteAsync() => Task.CompletedTask;
    }
}