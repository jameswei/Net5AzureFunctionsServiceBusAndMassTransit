namespace TestClient
{
    public interface ICommandFactory
    {
        ICommand Cmd(string text);
        ICommand Exit();
        ICommand Not(string info);
        ICommand Req(int number);
    }
}