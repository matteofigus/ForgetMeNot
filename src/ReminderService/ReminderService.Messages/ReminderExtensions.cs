using System;
using ReminderService.Messages;
using System.Collections.Generic;

namespace ReminderService.Messages
{
	public static class ReminderExtensions
	{
		public static ReminderMessage.DueReminder CreateFrom(this ReminderMessage.ScheduledReminderHasBeenJournaled source)
		{
			return new ReminderMessage.DueReminder (
				source.Reminder.ReminderId,
				source.Reminder.DeliveryUrl,
				source.Reminder.ContentType,
				source.Reminder.TimeoutAt,
				source.Reminder.Payload);
		}
	}
}

