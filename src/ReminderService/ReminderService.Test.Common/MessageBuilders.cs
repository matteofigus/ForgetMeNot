using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReminderService.Common;
using ReminderService.Messages;

namespace ReminderService.Test.Common
{
	public class MessageBuilders
	{
		public static IEnumerable<ReminderMessage.Schedule> BuildReminders(int count)
		{
			return Enumerable
				.Range (0, count)
				.Select (i => { 
				var reminderId = Guid.NewGuid ();
				return 
						new ReminderMessage.Schedule (
							reminderId,
							"http://deliveryUrl/" + i,
							"http://deadletterUrl",
							"application/json",
							SystemTime.Now ().AddMilliseconds(10),
							new FakePayload(reminderId).AsJsonEncoded());
			});
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
					new FakePayload(Guid.Empty).AsJsonEncoded()
				))
				.ToList();
		}

		public static IEnumerable<ReminderMessage.Cancel> BuildCancellationsAsSubsetOfReminders(int count, IEnumerable<ReminderMessage.Schedule> source)
		{
			return source
				.Select (r => new ReminderMessage.Cancel (r.ReminderId))
				.Take (count);
		}
	}
}

