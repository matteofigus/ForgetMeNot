using System;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core
{
	public class ScheduleReminderConsumer : IConsume<ReminderMessages.ScheduledReminderHasBeenJournaled>
	{
		public ScheduleReminderConsumer ()
		{
		}

		#region IConsume implementation

		public void Handle (ReminderMessages.ScheduledReminderHasBeenJournaled msg)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

