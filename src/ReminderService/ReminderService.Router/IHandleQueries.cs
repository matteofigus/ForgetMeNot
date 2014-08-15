using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router
{
	public interface IHandleQueries<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		TResponse Handle(TRequest request);
	}
}

