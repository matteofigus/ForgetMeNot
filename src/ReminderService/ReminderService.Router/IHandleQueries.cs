using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router
{
	public interface IHandleQueries<in TRequest, out TResponse> where TRequest : IRequest<TResponse>
	{
		TResponse Handle(TRequest request);
	}
}

