using System;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public static class Envelopes
	{
		public class Http<T> : IMessage where T : class, IMessage
		{
			public T Reminder { get; private set; }

			public Http (T toBeSent)
			{
				Ensure.NotNull(toBeSent, "toBeSent");
				Reminder = toBeSent;
			}
		}

		public class Amqp<T> : IMessage where T : class, IMessage
		{
			public T Reminder { get; private set; }

			public Amqp (T toBeSent)
			{
				Ensure.NotNull(toBeSent, "toBeSent");
				Reminder = toBeSent;
			}
		}

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
}

