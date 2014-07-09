using System;
using ReminderService.Router;

namespace ReminderService.Messages
{
	public static class ReminderMessages
	{
		public class ScheduleReminder : IMessage
		{
			public Guid ReminderId { get; set; }
			public string DeliveryUrl { get; private set; }
			public string DeadLetterUrl { get; private set; } //messages will be sent here if delivery to the DeliveryUrl fails.
			public string ContentType { get; private set; }
			public DateTime TimeoutAt { get; private set; }
			public byte[] Payload { get; private set; }

			public ScheduleReminder (string deliveryUrl, string contentType, DateTime timeoutAt, byte[] payload)
			{
				DeliveryUrl = deliveryUrl;
				ContentType = contentType;
				TimeoutAt = timeoutAt;
				Payload = payload;
			}
		}

		public class ScheduledReminderResponse : IMessage
		{
			public Guid ReminderId {get; set;}
		}

		public class ScheduledReminderHasBeenJournaled : IMessage
		{
			public ScheduleReminder Inner { get; private set; }
		}
	}
}

