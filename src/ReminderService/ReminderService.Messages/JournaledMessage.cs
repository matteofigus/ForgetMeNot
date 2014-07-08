using System;
using ReminderService.Common;
using ReminderService.Router;

namespace ReminderService.Messages
{
	public class JournaledMessage<T> where T : class, IMessage
	{
		private readonly T _inner;

		public T Message {
			get {return _inner;}
		}

		public JournaledMessage (T journaledMessage)
		{
			Ensure.NotNull (journaledMessage, "innerMessage");

			_inner = journaledMessage;
		}
	}
}

