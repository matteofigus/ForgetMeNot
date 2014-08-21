using System;
using ReminderService.Router;
using System.Collections.Generic;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public static class ReminderMessage
	{
		public abstract class ReminderMessageBase
		{
			public Guid ReminderId { get; set; }
		}

		public class Schedule : IMessage
		{
			public Guid ReminderId { get; set; }
			public string DeliveryUrl { get; set; }
			public string ContentType { get; set; }
			public DateTime TimeoutAt { get; set; }
			public byte[] Payload { get; set; }

			public Schedule ()
			{
				//default constructor
			}

			public Schedule (string deliveryUrl, string contentType, DateTime timeoutAt, byte[] payload)
			{
				DeliveryUrl = deliveryUrl;
				ContentType = contentType;
				TimeoutAt = timeoutAt;
				Payload = payload;
			}

			public Schedule (Guid reminderId, string deliveryUrl, string contentType, DateTime timeoutAt, byte[] payload)
				: this(deliveryUrl, contentType, timeoutAt, payload)
			{
				ReminderId = reminderId;
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

		public class Sent : IMessage
		{
			public Guid ReminderId { get; private set; }
			public DateTime SentStamp { get; private set; }

			public Sent (Guid reminderId, DateTime sentStamp)
			{
				Ensure.NotEmptyGuid(reminderId, "reminderId");

				ReminderId = reminderId;
				SentStamp = sentStamp;
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

