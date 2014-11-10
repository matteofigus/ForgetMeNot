using System;
using ReminderService.Messages;

namespace ReminderService.API.HTTP
{
	public class ReminderStatus
	{
		public string ReminderId { get; set;}
		public string Status { get; set; }
		public int RedeliveryAttempts { get; set; }
		public string DueAt { get; set; }
		public string GiveupAfter { get; set; }
		public int MaxRetries { get; set; }
		public string DeliveryUrl { get; set; }
		public string ContentType { get; set; }
		public string Encoding { get; set; }
		public string Transport { get; set;}
		public byte[] Payload { get; set; }

		public ReminderStatus ()
		{
			//default
		}

		public ReminderStatus (ReminderMessage.Schedule reminder, string status, int redeliveryAttempts)
		{
			ReminderId = reminder.ReminderId.ToString();
			DueAt = reminder.DueAt.ToString("O");
			GiveupAfter = reminder.GiveupAfter.HasValue ? reminder.GiveupAfter.Value.ToString("O") : string.Empty;
			MaxRetries = reminder.MaxRetries;
			DeliveryUrl = reminder.DeliveryUrl;
			ContentType = reminder.ContentType;
			Encoding = reminder.ContentEncoding.ToString();
			Transport = reminder.Transport.ToString();
			Payload = reminder.Payload;
			Status = status;
			RedeliveryAttempts = redeliveryAttempts;
		}
	}
}

