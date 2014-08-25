﻿using System;
using ReminderService.Common;
using ReminderService.Messages;
using System.Collections.Generic;

namespace ReminderService.Messages
{
	public static class ReminderExtensions
	{
		public static ReminderMessage.Due AsDue(this ReminderMessage.ISchedulable source)
		{
			if(source is ReminderMessage.Schedule)
				return new ReminderMessage.Due ((ReminderMessage.Schedule)source);

			throw new InvalidOperationException (string.Format("There is not support to convert from [{0}]", source.GetType().FullName));
		}

		public static ReminderMessage.Delivered AsSent(this ReminderMessage.Due source)
		{
			return AsSent (source, SystemTime.Now());
		}

		public static ReminderMessage.Delivered AsSent(this ReminderMessage.Due source, DateTime sentStamp)
		{
			return new ReminderMessage.Delivered (source.ReminderId, sentStamp);
		}

		public static bool DoNotAttemptRedelivery(this ReminderMessage.Undelivered undelivered)
		{
			return !undelivered.Reminder.GiveupAfter.HasValue;
		}
	}
}

