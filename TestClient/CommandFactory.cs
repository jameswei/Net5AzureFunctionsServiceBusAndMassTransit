using System;

namespace TestClient
{
    public class CommandFactory
        : ICommandFactory
    {
        readonly Func<SendCommandCommand> _sendCommandCommandFactory;
        readonly Func<SendNotificationCommand> _sendNotificationCommandFactory;
        readonly Func<SendRequestCommand> _sendRequestCommandFactory;

        public CommandFactory(Func<SendRequestCommand> sendRequestCommandFactory,
            Func<SendNotificationCommand> sendNotificationCommandFactory,
            Func<SendCommandCommand> sendCommandCommandFactory)
        {
            _sendRequestCommandFactory = sendRequestCommandFactory;
            _sendNotificationCommandFactory = sendNotificationCommandFactory;
            _sendCommandCommandFactory = sendCommandCommandFactory;
        }

        public ICommand Cmd(string text)
        {
            var result = _sendCommandCommandFactory();
            result.Cmd.Text = text;
            return result;
        }

        public ICommand Exit() => new ExitCommand();

        public ICommand Not(string info)
        {
            var result = _sendNotificationCommandFactory();
            result.Notification.Info = info;
            return result;
        }

        public ICommand Req(int number)
        {
            var result = _sendRequestCommandFactory();
            result.Request.Number = number;
            return result;
        }
    }
}