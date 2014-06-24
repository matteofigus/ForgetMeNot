using System;

namespace ReminderService.Router
{
	internal interface IDispatchMessages
	{
		void Dispatch(IMessage message);
	}

	internal class MessageDispatcher<T> : IDispatchMessages where T : class, IMessage
	{
		private readonly IConsume<T> _consumer;

		public MessageDispatcher(IConsume<T> consumer)
		{
			if (consumer == null)
				throw new ArgumentNullException("consumer");
			_consumer = consumer;
		}

		public void Dispatch(IMessage message)
		{
			var msg = message as T;
			if (msg != null)
			{
				_consumer.Handle(msg);
			}
			//do we need to log as an 'else' here? otherwise, have we swallowed a case that can cause bugs?
		}
	}
}

