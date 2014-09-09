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
				(ReminderMessage.ContentEncodingEnum)Enum.Parse(typeof(ReminderMessage.ContentEncodingEnum), source.Encoding.ToLower()),
				(ReminderMessage.TransportEnum)Enum.Parse(typeof(ReminderMessage.TransportEnum), source.Transport.ToLower()), 
				source.Payload,
				source.MaxRetries,
				source.GiveupAfter
			);

			return schedule;
		}
	}
}

