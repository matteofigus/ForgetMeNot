using System;
using ReminderService.Messages;
using System.Text;

namespace ReminderService.API.HTTP.Models
{
	public static class ExtensionMethods
	{
		public static ReminderMessage.Schedule BuildScheduleMessage(this ScheduleReminder source, Guid reminderId)
		{
			var schedule = new ReminderMessage.Schedule (
				reminderId,
				source.DueAt,
				source.DeliveryUrl,
				source.ContentType,
				source.Encoding,
				(ReminderMessage.TransportEnum)Enum.Parse(typeof(ReminderMessage.TransportEnum), source.Transport.ToLower()), //we validated the user request, so this should not be able to fail
				source.Payload,
				source.MaxRetries,
				source.GiveupAfter
			);

			return schedule;
		}
	}
}

