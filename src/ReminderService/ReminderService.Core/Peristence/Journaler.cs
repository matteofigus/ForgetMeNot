using System;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;

namespace ReminderService.Core
{
	/// <summary>
	/// Journaler writes all incoming messages to a persistent journal. Publishes a JournaledMessage<c></c> that can be consumed
	/// by other interested components once a message has been journaled.
	/// </summary>
	public class Journaler : IConsume<ReminderMessages.ScheduleReminder> //todo: write a QueuedConsumer<T> and use it here
	{
		// can this class implement ISubscribe<T> as a way to wire-up / route messages between components?
		// can delegate to an inner IBus instance for sending messages to subscribers?
		private readonly IPublish _bus;

		public Journaler (IPublish bus)
		{
			Ensure.NotNull (bus, "bus");
			_bus = bus;
		}

		#region IConsume implementation

		public void Handle (ReminderMessages.ScheduleReminder msg)
		{
			//write message to persistence
			IMessage journaledEvent = new ReminderMessages.ScheduledReminderHasBeenJournaled (msg) as IMessage;
			_bus.Publish (journaledEvent);
		}

		#endregion

	}
}

