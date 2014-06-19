namespace ReminderService.Router
{
    public interface IConsume<in T> where T : IMessage
    {
        void Handle(T msg);
    }
}
