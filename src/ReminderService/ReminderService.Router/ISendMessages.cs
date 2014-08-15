using ReminderService.Common;

namespace ReminderService.Router
{
    public interface ISendMessages
    {
        void Send(IMessage message);
    }
}
