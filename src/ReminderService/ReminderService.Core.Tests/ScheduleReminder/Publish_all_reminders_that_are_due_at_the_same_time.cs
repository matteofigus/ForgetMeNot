using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.Tests.ScheduleReminder
{
	[TestFixture ()]
	public class Publish_all_reminders_that_are_due_at_the_same_time : Scheduler_Spec
	{
		[TestFixtureSetUp]
		public void given_there_are_some_reminders_due_at_the_same_time()
		{
			SetNow (SystemTime.UtcNow ());
			GivenA (new SystemMessage.Start ());

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now,
					"http://deliveryUrl/1",
					"content/type",
					new byte[0])));

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now,
					"http://deliveryUrl/2",
					"content/type",
					new byte[0])));

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now.AddMilliseconds (25),
					"http://deliveryUrl/3",
					"content/type",
					new byte[0])));

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now,
					"http://deliveryUrl/4",
					"content/type",
					new byte[0])));

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now.AddMilliseconds (25),
					"http://deliveryUrl/3",
					"content/type",
					new byte[0])));
		}

		[Test]
		public void should_publish_all_reminders_that_are_due_at_the_same_time()
		{
			FireTimer ();
			Assert.AreEqual (3, Received.Count);

			AdvanceTimeBy (26);
			FireTimer ();
			Assert.AreEqual (5, Received.Count);
		}
	}
}

