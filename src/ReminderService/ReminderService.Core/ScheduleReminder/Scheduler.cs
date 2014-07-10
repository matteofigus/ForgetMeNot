using System;
using ReminderService.DataStructures;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core.ScheduleReminder
{
	public class Scheduler : IConsume<ReminderMessages.ScheduledReminderHasBeenJournaled>
	{
		private readonly ITimer _timer;
		private readonly PriorityQueue<ScheduledReminder> _reminders;

		public Scheduler (ITimer timer)
		{
			Ensure.NotNull (timer, "timer");
		}
			
		public void Handle (ReminderMessages.ScheduledReminderHasBeenJournaled msg)
		{
			throw new NotImplementedException ();
		}

	}
}

