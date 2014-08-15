using ReminderService.Common;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Messages
{
	public static class DeliveryEnvelope
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
	}
}

