using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.Tests.ScheduleReminder
{
	[TestFixture ()]
	public class Publishing_messages_as_they_become_due : Scheduler_Spec
	{
		[TestFixtureSetUp]
		public void given_there_are_some_reminders()
		{
			SetNow(SystemTime.UtcNow());
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
					Now.AddMilliseconds(15),
					new byte[0])));

			GivenA (new JournaledEnvelope<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					"http://deliveryUrl/3",
					"content/type",
					Now.AddMilliseconds(25),
					new byte[0])));

			GivenA (new JournaledEnvelope<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					"http://deliveryUrl/4",
					"content/type",
					Now.AddMilliseconds(100),
					new byte[0])));
		}

		[Test]
		public void should_publish_messages_as_they_become_due()
		{
			FireTimer ();
			Assert.AreEqual (1, Received.Count);

			AdvanceTimeBy (17);
			FireTimer ();
			Assert.AreEqual (2, Received.Count);

			AdvanceTimeBy (25);
			FireTimer ();
			Assert.AreEqual (3, Received.Count);

			AdvanceTimeBy (101);
			FireTimer ();
			Assert.AreEqual (4, Received.Count);
		}
	}
}

