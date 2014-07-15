using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.Tests
{
	[TestFixture]
	public class When_not_started : Scheduler_Spec
	{
		private DateTime now;

		[TestFixtureSetUp]
		public void given_there_are_some_reminders()
		{
			now = SystemTime.Now();
			var reminder = new ReminderMessages.ScheduleReminder (
				"http://deliveryUrl",
				"content/type",
				SystemTime.Now (),
				new byte[0]);
			var journaledReminder = new ReminderMessages.ScheduledReminderHasBeenJournaled (reminder);

			GivenA (journaledReminder);
		}

		[SetUp]
		public void when_they_are_due()
		{
			SystemTime.Set (now);
			FireTimer ();
		}

		[Test]
		public void then_nothing_should_happen()
		{
			Assert.AreEqual (0, Received.Count);
		}
	}
}

