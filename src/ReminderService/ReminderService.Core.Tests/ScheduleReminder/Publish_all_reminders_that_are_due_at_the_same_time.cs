using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.Tests
{
	[TestFixture ()]
	public class Publish_all_reminders_that_are_due_at_the_same_time : Scheduler_Spec
	{
		[TestFixtureSetUp]
		public void given_there_are_some_reminders_due_at_the_same_time()
		{
			SetNow (SystemTime.Now ());
			GivenA (new SystemMessage.Start ());

			GivenA (new JournaledEnvelope<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					"http://deliveryUrl/1",
					"content/type",
					Now,
					new byte[0])));

			GivenA (new JournaledEnvelope<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					"http://deliveryUrl/2",
					"content/type",
					Now,
					new byte[0])));

			GivenA (new JournaledEnvelope<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					"http://deliveryUrl/3",
					"content/type",
					Now.AddMilliseconds (25),
					new byte[0])));

			GivenA (new JournaledEnvelope<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					"http://deliveryUrl/4",
					"content/type",
					Now,
					new byte[0])));

			GivenA (new JournaledEnvelope<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					"http://deliveryUrl/3",
					"content/type",
					Now.AddMilliseconds (25),
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

