using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.Tests
{
	[TestFixture ()]
	public class Publishing_messages_as_they_become_due : Scheduler_Spec
	{
		private DateTime now;

		[TestFixtureSetUp]
		public void given_there_are_some_reminders()
		{
			now = SystemTime.Now();
			GivenA (new SystemMessage.Start ());

			GivenA (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/1",
					"content/type",
					now,
					new byte[0])));

			GivenA (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/2",
					"content/type",
					now.AddMilliseconds(15),
					new byte[0])));

			GivenA (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/3",
					"content/type",
					now.AddMilliseconds(25),
					new byte[0])));

			GivenA (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/4",
					"content/type",
					now.AddMilliseconds(100),
					new byte[0])));
		}

		[Test]
		public void should_publish_messages_as_they_become_due()
		{
			FireTimer (now);
			Assert.AreEqual (1, Received.Count);

			//SystemTime.Set (now.AddMilliseconds (17));
			FireTimer (now.AddMilliseconds (17));
			Assert.AreEqual (2, Received.Count);

			//SystemTime.Set (now.AddMilliseconds (25));
			FireTimer (now.AddMilliseconds (25));
			Assert.AreEqual (3, Received.Count);

			//SystemTime.Set (now.AddMilliseconds (101));
			FireTimer (now.AddMilliseconds (101));
			Assert.AreEqual (4, Received.Count);
		}
	}
}

