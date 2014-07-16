using System;
using ReminderService.Common;
using ReminderService.Router;
using System.Collections.Generic;

namespace ReminderService.Messages
{
	public static class ReminderMessage
	{
		public class Schedule : IMessage
		{
			public Guid ReminderId { get; set; }
			public string DeliveryUrl { get; private set; }
			public string DeadLetterUrl { get; private set; } //messages will be sent here if delivery to the DeliveryUrl fails.
			public string ContentType { get; private set; }
			public DateTime TimeoutAt { get; private set; }
			public byte[] Payload { get; private set; }

			public Schedule (string deliveryUrl, string contentType, DateTime timeoutAt, byte[] payload)
			{
				DeliveryUrl = deliveryUrl;
				ContentType = contentType;
				TimeoutAt = timeoutAt;
				Payload = payload;
			}
		}

		public class ScheduledResponse : IMessage
		{
			public Guid ReminderId {get; set;}
		}

		public class Due : IMessage
		{
			public Guid ReminderId { get; set; }
			public string DeliveryUrl { get; private set; }
			public string DeadLetterUrl { get; private set; } //messages will be sent here if delivery to the DeliveryUrl fails.
			public string ContentType { get; private set; }
			public DateTime TimeoutAt { get; private set; }
			public byte[] Payload { get; private set; }

			public Due (Guid reminderId, string deliveryUrl, string contentType, DateTime timeoutAt, byte[] payload)
			{
				ReminderId = reminderId;
				DeliveryUrl = deliveryUrl;
				ContentType = contentType;
				TimeoutAt = timeoutAt;
				Payload = payload;
			}
		}

		public class Cancel : IMessage
		{
			public Guid ReminderId { get; set; }

			public Cancel (Guid reminderId)
			{
				ReminderId = reminderId;
			}
		}
			
		public class DueReminderNotCanceled : IMessage
		{
			public Guid ReminderId { get; set; }
			public string DeliveryUrl { get; private set; }
			public string DeadLetterUrl { get; private set; }
			public string ContentType { get; private set; }
			public DateTime TimeoutAt { get; private set; }
			public byte[] Payload { get; private set; }

			public DueReminderNotCanceled (Guid reminderId, string deliveryUrl, string contentType, DateTime timeoutAt, byte[] payload)
			{
				ReminderId = reminderId;
				DeliveryUrl = deliveryUrl;
				ContentType = contentType;
				TimeoutAt = timeoutAt;
				Payload = payload;
			}

			public static DueReminderNotCanceled CreateFrom (Due due)
			{
				return new DueReminderNotCanceled (
					due.ReminderId,
					due.DeliveryUrl,
					due.ContentType,
					due.TimeoutAt,
					due.Payload
				);
			}
		}

		public class EqualityComparer<T> : IEqualityComparer<T> where T : IMessage
		{
			private readonly Func<T, int> _getHashCode;
			private readonly Func<T, T, bool> _equals;

			public EqualityComparer (Func<T, int> getHashCodeDelegate, Func<T, T, bool> equalsDelegate)
			{
				Ensure.NotNull(getHashCodeDelegate, "getHashCodeDelegate");
				Ensure.NotNull(equalsDelegate, "equalsDelegate");

				_getHashCode = getHashCodeDelegate;
				_equals = equalsDelegate;
			}

			public bool Equals (T x, T y)
			{
				return _equals (x, y);
			}

			public int GetHashCode (T obj)
			{
				return _getHashCode (obj);
			}
		}
	}
}

