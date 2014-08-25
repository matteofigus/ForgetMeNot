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
		public static IEnumerable<ReminderMessage.Schedule> BuildReminders(int count, int maxAttempts = 1, DateTime? giveupAfter = null)
		{
			return Enumerable
				.Range (0, count)
				.Select (i => { 
				var reminderId = Guid.NewGuid ();
				return 
						new ReminderMessage.Schedule (
							reminderId,
							SystemTime.UtcNow ().AddMilliseconds(10),
							"http://deliveryUrl/" + i,
							"application/json",
							new TestPayload(reminderId).AsJsonEncoded(),
							maxAttempts,
							giveupAfter
						);
			});
		}

		public static IEnumerable<ReminderMessage.Schedule> BuildRemindersWithoutIds(int count, int maxAttempts = 1, DateTime? giveupAfter = null)
		{
			return Enumerable
				.Range (0, count)
				.Select (i => 
					new ReminderMessage.Schedule (
						SystemTime.UtcNow ().AddMilliseconds (100),
						"http://deliveryUrl/" + i,
						"application/json",
						new TestPayload (Guid.NewGuid ()).AsJsonEncoded (),
						maxAttempts,
						giveupAfter
			));
			//.ToList();
		}

		public static IEnumerable<ReminderMessage.Cancel> BuildCancellationsAsSubsetOfReminders(int count, IEnumerable<ReminderMessage.Schedule> source)
		{
			return source
				.Select (r => new ReminderMessage.Cancel (r.ReminderId))
				.Take (count);
		}
	}
}

