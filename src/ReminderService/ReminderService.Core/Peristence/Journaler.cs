using System;
using OpenTable.Services.Components.Logging;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;

namespace ReminderService.Core
{
	/// <summary>
	/// Journaler writes all incoming messages to a persistent journal. Publishes a JournaledMessage that can be consumed
	/// by other interested components once a message has been journaled.
	/// </summary>
	public class Journaler : IConsume<ReminderMessage.Schedule> //todo: write a QueuedConsumer<T> and use it here
	{
		// can this class implement ISubscribe<T> as a way to wire-up / route messages between components?
		// can delegate to an inner IBus instance for sending messages to subscribers?

		private readonly IPublish _bus;
		private readonly IJournaler _journaler;
		private readonly ILogger _logger;

		public Journaler (IPublish bus, IJournaler journaler)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (journaler, "journaler");

			_bus = bus;
			_journaler = journaler;
		}

		public void Handle (ReminderMessage.Schedule msg)
		{
			try {
				_journaler.Write (msg);
				IMessage journaledEvent = new ReminderMessage.ScheduledHasBeenJournaled (msg) as IMessage;
				_bus.Publish (journaledEvent);
			}
			catch (Exception ex) {
				_logger.LogException (Level.Error, ex, "Exception while attempting to write to journal.");
				throw;
			}
		}

	}
}

