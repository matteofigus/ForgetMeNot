using System;
using ReminderService.Common;

namespace ReminderService.Router
{
	public interface IRequest<TResponse> : IMessage {}

	public interface IHandleQueries<TRequest, TResponse> where TRequest : IRequest<TResponse>
	{
		TResponse Handle(TRequest request);
	}
}

