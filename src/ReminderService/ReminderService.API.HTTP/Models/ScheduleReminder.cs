using System;

namespace ReminderService.API.HTTP.Models
{
	public class ScheduleReminder
	{
		public DateTime DueAt { get; set; }
		public DateTime? GiveupAfter { get; set; }
		public int MaxRetries { get; set; }
		public string DeliveryUrl { get; set; }
		public string ContentType { get; set; }
		public string Encoding { get; set; }
		public string Transport { get; set;}
		public byte[] Payload { get; set; }

		public ScheduleReminder ()
		{
			//empty
		}

		public ScheduleReminder (DateTime dueAt, string deliveryUrl, string contentType, string encoding, string transport, byte[] payload, int maxRetries, DateTime? giveupAfter = null)
		{
			DueAt = dueAt;
			DeliveryUrl = deliveryUrl;
			ContentType = contentType;
			Encoding = encoding;
			Transport = transport;
			Payload = payload;
			MaxRetries = maxRetries;
			GiveupAfter = giveupAfter;
		}
	}
}

