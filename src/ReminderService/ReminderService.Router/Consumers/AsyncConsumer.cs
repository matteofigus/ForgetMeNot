using System;
using ReminderService.Common;
using System.Threading.Tasks;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router
{
	public class AsyncConsumer<T> : IConsume<T> where T : IMessage
	{
		private readonly IConsume<T> _inner;

		public AsyncConsumer (IConsume<T> inner)
		{
			Ensure.NotNull (inner, "inner");

			_inner = inner;
		}

		public void Handle (T msg)
		{
			Task.Run(() => _inner.Handle(msg));
		}
	}
}

