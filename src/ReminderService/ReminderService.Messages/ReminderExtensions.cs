using System;
using ReminderService.Messages;
using System.Collections.Generic;

namespace ReminderService.Messages
{
	public static class ReminderExtensions
	{
		public static ReminderMessage.Due CreateFrom(this ReminderMessage.ScheduledHasBeenJournaled source)
		{
			return new ReminderMessage.Due (
				source.Reminder.ReminderId,
				source.Reminder.DeliveryUrl,
				source.Reminder.ContentType,
				source.Reminder.TimeoutAt,
				source.Reminder.Payload);
		}
	}
}

