namespace ReminderService.Router
{
    public interface IBus : ISendMessages, ISubscribe, ISubscribeToQueries
    {
    }
}
