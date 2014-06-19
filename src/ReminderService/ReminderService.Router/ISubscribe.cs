namespace ReminderService.Router
{
    public interface ISubscribe
    {
        void SubscribeTo<T>(IConsume<T> handler) where T : IMessage;
        void UnSubscribeTo<T>(IConsume<T> handler) where T : IMessage;
    }
}
