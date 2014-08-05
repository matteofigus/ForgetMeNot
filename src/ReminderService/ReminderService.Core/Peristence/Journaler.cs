using System;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;
using log4net;

namespace ReminderService.Core
{
	/// <summary>
	/// Journaler writes all incoming messages to a persistent journal. Publishes a JournaledMessage that can be consumed
	/// by other interested components once a message has been journaled.
	/// </summary>
	public class Journaler : 
		IConsume<ReminderMessage.Schedule>,
		IConsume<ReminderMessage.Cancel>,
		IConsume<ReminderMessage.Sent>
	{
		//todo: write a QueuedConsumer<T> and use it here
		// can this class implement ISubscribe<T> as a way to wire-up / route messages between components?
		// can delegate to an inner IBus instance for sending messages to subscribers?

		private readonly IPublish _bus;
		private readonly IJournalEvents _journaler;
		private static readonly ILog Logger = LogManager.GetLogger(typeof(Journaler));

		public Journaler (IPublish bus, IJournalEvents journaler)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (journaler, "journaler");

			_bus = bus;
			_journaler = journaler;
		}

		public void Handle (ReminderMessage.Schedule msg)
		{
			WriteToJournal (msg);
			SendOnBus (new JournaledEnvelope<ReminderMessage.Schedule> (msg) as IMessage);
		}

		public void Handle(ReminderMessage.Cancel msg)
		{
			WriteToJournal (msg);
			SendOnBus (new JournaledEnvelope<ReminderMessage.Cancel> (msg) as IMessage);
		}

		public void Handle(ReminderMessage.Sent msg)
		{
			WriteToJournal (msg);
		}

		private void WriteToJournal(IMessage msg)
		{
			try {
				_journaler.Write (msg);
			}
			catch (Exception ex) {
				Logger.Error ("Exception while attempting to write to journal.", ex);
				throw;
			}
		}

		private void SendOnBus(IMessage msg)
		{
			try {
				_bus.Publish (msg);
			}
			catch (Exception ex) {
				Logger.Error ("Exception while attempting to send on the bus.", ex);
				throw;
			}
		}
	}
}

