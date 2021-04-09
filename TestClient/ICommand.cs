using System.Threading.Tasks;

namespace TestClient
{
    public interface ICommand
    {
        Task ExecuteAsync();
    }
}