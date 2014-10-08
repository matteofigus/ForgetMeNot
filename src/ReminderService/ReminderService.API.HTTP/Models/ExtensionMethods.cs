using System;
using ReminderService.Messages;
using System.Text;
using NodaTime;
using NodaTime.Text;

namespace ReminderService.API.HTTP.Models
{
	public static class ExtensionMethods
	{
		public static ReminderMessage.Schedule BuildScheduleMessage(this ScheduleReminder source, Guid reminderId)
		{
			var schedule = new ReminderMessage.Schedule (
				reminderId,
				source.DueAt.AsNodaTimeUtcInstant().ToDateTimeUtc(),
				source.DeliveryUrl,
				source.ContentType,
				(ReminderMessage.ContentEncodingEnum)Enum.Parse(typeof(ReminderMessage.ContentEncodingEnum), source.Encoding.ToLower()),
				(ReminderMessage.TransportEnum)Enum.Parse(typeof(ReminderMessage.TransportEnum), source.Transport.ToLower()), 
				source.Payload,
				source.MaxRetries,
				string.IsNullOrEmpty(source.GiveupAfter) ? 
					new Nullable<DateTime>() : 
					new Nullable<DateTime>(source.GiveupAfter.AsNodaTimeUtcInstant().ToDateTimeUtc())
			);

			return schedule;
		}

		public static ZonedDateTime AsNodaTimeUtcInstant(this string dateTimeString)
		{
			var pattern = OffsetDateTimePattern.ExtendedIsoPattern;
			return pattern.Parse (dateTimeString).GetValueOrThrow().ToInstant().InUtc();
		}

		public static MonitorGroup AddMonitor(this MonitorGroup group, DateTime timeStamp, string key, string value)
		{
			group.Upsert (new MonitorItem {
				TimeStamp = timeStamp,
				Key = key,
				Value = value
			});

			return group;
		}
	}
}

