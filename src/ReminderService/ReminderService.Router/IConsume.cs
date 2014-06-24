namespace ReminderService.Router
{
	public interface IConsume<T> where T : IMessage
    {
        void Handle(T msg);
    }
}
