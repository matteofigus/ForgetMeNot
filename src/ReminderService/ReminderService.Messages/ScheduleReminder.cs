using System;
using ReminderService.Router;

namespace ReminderService.Messages
{
	public interface IScheduleReminder : IMessage
	{
		Guid ReminderId { get; set; }
		string DeliveryUrl { get; }
		string ContentType { get; }
		DateTime TimeoutAt { get; }
		byte[] Payload { get; }
	}

	public interface IScheduledReminderResponse : IMessage
	{
		Guid ReminderId { get; set; }
	}

	public class ScheduleReminder : IScheduleReminder
	{
		public Guid ReminderId { get; set;}
		public string DeliveryUrl { get; private set; }
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

	public class ScheduledReminderResponse : IScheduledReminderResponse
	{
		public Guid ReminderId {get; set;}
	}
}

