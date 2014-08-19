using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router
{
    public interface ISendQueries
    {
        TResponse Send<TResponse>(IRequest<TResponse> query);
    }
}