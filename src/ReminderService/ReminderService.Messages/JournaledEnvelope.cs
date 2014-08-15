using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public class JournaledEnvelope<T> : IMessage where T : class, IMessage
	{
		private readonly T _inner;

		public T Message {
			get {return _inner;}
		}

		public JournaledEnvelope (T journaledMessage)
		{
			Ensure.NotNull (journaledMessage, "innerMessage");

			_inner = journaledMessage;
		}
	}
}

