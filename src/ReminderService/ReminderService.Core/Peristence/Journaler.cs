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
	public class Journaler : IConsume<ScheduleReminder>
	{
		private readonly IPublish _bus;

		public Journaler (IPublish bus)
		{
			Ensure.NotNull (bus, "bus");
			_bus = bus;
		}

		#region IConsume implementation

		public void Handle (ScheduleReminder msg)
		{
			//write message to persistence
			IMessage journaled = new JournaledMessage<ScheduleReminder> (msg) as IMessage;
			_bus.Publish (journaled);
		}

		#endregion

	}
}

