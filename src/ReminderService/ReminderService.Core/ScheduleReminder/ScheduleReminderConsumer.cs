using System;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core
{
	public class ScheduleReminderConsumer : IConsume<ScheduleReminder>
	{
		public ScheduleReminderConsumer ()
		{
		}

		#region IConsume implementation

		public void Handle (ScheduleReminder scheduleReminder)
		{
			Ensure.NotNull (scheduleReminder, "scheduleReminder");

		}

		#endregion
	}
}

