﻿using System;
using ReminderService.Router;
using System.Collections.Generic;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public static class ReminderMessage
	{
		public interface IReminder : IMessage
		{
			Guid ReminderId { get; set; }
		}

		public interface IDeliverable : IReminder
		{
			string DeliveryUrl { get; set; }
			string ContentType { get; set; }
			byte[] Payload { get; set; }
		}

		public interface ISchedulable : IReminder
		{
			DateTime DueAt { get; set; }
			DateTime? GiveupAfter { get; set; }
			DateTime? RescheduleFor { get; set; }
		}

		public class Schedule : ISchedulable, IDeliverable, IReminder
		{
			public Guid ReminderId { get; set; }
			public DateTime DueAt { get; set; }
			public DateTime? GiveupAfter { get; set; }
			public int FirstWaitDurationMs { get; set; }
			public DateTime? RescheduleFor { get; set; }
			public string DeliveryUrl { get; set; }
			public string ContentType { get; set; }
			public byte[] Payload { get; set; }

			public Schedule ()
			{
				//default constructor
			}

			public Schedule (DateTime dueAt, string deliveryUrl, string contentType, byte[] payload, int firstWaitDurationMs, DateTime? giveupAfter = null, DateTime? rescheduleFor = null)
			{
				DueAt = dueAt;
				FirstWaitDurationMs = firstWaitDurationMs;
				GiveupAfter = giveupAfter;
				RescheduleFor = rescheduleFor;
				DeliveryUrl = deliveryUrl;
				ContentType = contentType;
				Payload = payload;
			}

			public Schedule (Guid reminderId, DateTime dueAt, string deliveryUrl, string contentType, byte[] payload, int firstWaitDurationMs, DateTime? giveupAfter = null, DateTime? rescheduleFor = null)
				: this(dueAt, deliveryUrl, contentType, payload, firstWaitDurationMs, giveupAfter, rescheduleFor)
			{
				ReminderId = reminderId;
			}
		}

		public class Due : IReminder
		{
			public Guid ReminderId { get; set; }
			public ReminderMessage.Schedule Reminder { get; set; }

			public Due (ReminderMessage.Schedule due)
			{
				Reminder = due;
				ReminderId = due.ReminderId;
			}
		}

		public class Cancel : IReminder
		{
			public Guid ReminderId { get; set; }

			public Cancel (Guid reminderId)
			{
				ReminderId = reminderId;
			}
		}

		public class Delivered : IReminder
		{
			public Guid ReminderId { get; set; }
			public DateTime SentStamp { get; private set; }

			public Delivered (Guid reminderId, DateTime sentStamp)
			{
				Ensure.NotEmptyGuid(reminderId, "reminderId");

				ReminderId = reminderId;
				SentStamp = sentStamp;
			}
		}

		public class Undeliverable : IReminder
		{
			public Guid ReminderId { get; set; }
			public ReminderMessage.Schedule Reminder { get; set; }
			public string Reason { get; set; }

			public Undeliverable (ReminderMessage.Schedule reminder, string reason)
			{
				Reminder = reminder;
				Reason = reason;
				ReminderId = reminder.ReminderId;
			}
		}

		public class SentToDeadLetter : Delivered
		{
			public SentToDeadLetter (Guid reminderId, DateTime sentStamp) 
				: base (reminderId, sentStamp)
			{
				//empty	
			}
		}

		public class Undelivered : IReminder
		{
			public Guid ReminderId { get; set; }
			public ReminderMessage.Schedule Reminder { get; set; }
			public string Reason { get; set; }

			public Undelivered (ReminderMessage.Schedule reminder, string reason)
			{
				Reminder = reminder;
				Reason = reason;
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

