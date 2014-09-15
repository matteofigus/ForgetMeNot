using System;
using System.Collections.Generic;
using ReminderService.API.HTTP.Models;
using System.Linq;
using ReminderService.Common;
using ReminderService.Test.Common;
using ReminderService.Messages;

namespace ReminderService.API.HTTP.Tests
{
	public class Helpers
	{
		public static IEnumerable<ScheduleReminder> BuildScheduleRequests(int count, int maxAttempts = 1, DateTime? giveupAfter = null)
		{
			return Enumerable
				.Range (0, count)
				.Select (i => 
					new ScheduleReminder (
						SystemTime.UtcNow ().AddSeconds (100).ToString("o"), //yyyy-MM-ddTHH:mm:ssZ
						"http://deliveryUrl/" + i,
						"application/json",
						"utf8",
						"http",
						new TestPayload (Guid.NewGuid ()).AsJsonEncoded (),
						maxAttempts,
						giveupAfter.ToString()
					));
		}

		public static ScheduleReminder BuildScheduleRequest(Guid reminderId, int maxAttempts = 1, DateTime? giveupAfter = null)
		{
			return new ScheduleReminder (
				SystemTime.UtcNow ().AddSeconds (100).ToString("o"),
				"http://deliveryUrl/" + reminderId,
				"application/json",
				"utf8",
				"http",
				new TestPayload (reminderId).AsJsonEncoded (),
				maxAttempts,
				giveupAfter.ToString()
			);
		}
	}
}

