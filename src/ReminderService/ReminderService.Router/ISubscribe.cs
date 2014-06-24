namespace ReminderService.Router
{
    public interface ISubscribe
    {
		void Subscribe<T>(IConsume<T> handler) where T : class, IMessage;
		void UnSubscribe<T>(IConsume<T> handler) where T : IMessage;
    }
}
