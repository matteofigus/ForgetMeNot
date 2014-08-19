namespace ReminderService.Router
{
    public interface IBus : ISendMessages, ISendQueries, ISubscribe, ISubscribeToQueries
    {
    }
}
