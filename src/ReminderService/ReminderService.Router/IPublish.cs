using ReminderService.Common;

namespace ReminderService.Router
{
    public interface IPublish
    {
        void Publish(IMessage message);
    }
}
