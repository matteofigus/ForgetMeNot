using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.Tests.ScheduleReminder
{
	[TestFixture]
	public class When_not_started : Scheduler_Spec
	{
		[TestFixtureSetUp]
		public void given_there_are_some_reminders()
		{
			SetNow(SystemTime.Now());
			var reminder = new ReminderMessage.Schedule (
				Now,
				"http://deliveryUrl",
				"content/type",
				new byte[0],
				0);
			var journaledReminder = new Envelopes.Journaled<ReminderMessage.Schedule> (reminder);

			GivenA (journaledReminder);
		}

		[SetUp]
		public void when_they_are_due()
		{
			FireTimer ();
		}

		[Test]
		public void then_nothing_should_happen()
		{
			Assert.AreEqual (0, Received.Count);
		}
	}
}

