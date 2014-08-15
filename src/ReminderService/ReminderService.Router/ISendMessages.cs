using ReminderService.Router.MessageInterfaces;


namespace ReminderService.Router
{
    public interface ISendMessages
    {
        void Send(IMessage message);
    }
}
