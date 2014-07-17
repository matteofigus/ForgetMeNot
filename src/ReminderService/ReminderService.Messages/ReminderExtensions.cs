using System;
using ReminderService.Messages;
using System.Collections.Generic;

namespace ReminderService.Messages
{
	public static class ReminderExtensions
	{
		public static ReminderMessage.Due DueReminder(this ReminderMessage.Schedule source)
		{
			return new ReminderMessage.Due (
				source.ReminderId,
				source.DeliveryUrl,
				source.DeadLetterUrl,
				source.ContentType,
				source.TimeoutAt,
				source.Payload);
		}
	}
}

