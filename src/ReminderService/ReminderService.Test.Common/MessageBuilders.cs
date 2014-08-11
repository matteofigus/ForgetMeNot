using System;
using System.Linq;
using ReminderService.Messages;
using System.Collections.Generic;
using ReminderService.Common;
using System.Text;

namespace ReminderService.Test.Common
{
	public class MessageBuilders
	{
		public static IEnumerable<ReminderMessage.Schedule> BuildReminders(int count)
		{
			return Enumerable
				.Range (0, count)
				.Select (i => new ReminderMessage.Schedule (
					Guid.NewGuid(),
					"deliveryUrl",
					"deadletterUrl",
					"application/json",
					SystemTime.Now(),
					Encoding.UTF8.GetBytes("{\"property1\": \"value1\"}")
				));
		}

		public static IEnumerable<ReminderMessage.Schedule> BuildRemindersWithoutIds(int count)
		{
			return Enumerable
				.Range (0, count)
				.Select (i => new ReminderMessage.Schedule (
					"http://deliveryUrl/" + i,
					"http://deadletterUrl",
					"application/json",
					SystemTime.Now().AddMilliseconds(100),
					Encoding.UTF8.GetBytes("{\"property1\": \"value1\"}")
				));
		}

		public static IEnumerable<IMessage> BuildCancellationsAsSubsetOfReminders(int count, IEnumerable<ReminderMessage.Schedule> source)
		{
			return source
				.Select (r => new ReminderMessage.Cancel (r.ReminderId))
				.Take (count);
		}
	}
}

