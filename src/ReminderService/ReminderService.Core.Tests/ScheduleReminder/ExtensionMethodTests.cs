using NUnit.Framework;
using System;
using System.Linq;
using ReminderService.DataStructures;
using ReminderService.Core.ScheduleReminder;

namespace ReminderService.Core.Tests.ScheduleReminder
{
	[TestFixture ()]
	public class ExtensionMethodTests
	{
		[Test ()]
		public void GetRemindersAtTime ()
		{
			var now = DateTime.Now;
			var pq = new MinPriorityQueue<ScheduledReminder>(4);
			pq.Insert(new ScheduledReminder(
				Guid.NewGuid(),
				now,
				new object(), 
				new Uri("http://some/place")
			));
			pq.Insert(new ScheduledReminder(
				Guid.NewGuid(),
				now,
				new object(), 
				new Uri("http://some/place")));
			pq.Insert(new ScheduledReminder(
				Guid.NewGuid(),
				now.AddHours(+1),
				new object(), 
				new Uri("http://some/place")));

			Assert.AreEqual(3, pq.Size);

			var dueReminders = pq.GetRemindersAtTime(now);

			Assert.AreEqual (2, dueReminders.Count());
		}
	}
}

