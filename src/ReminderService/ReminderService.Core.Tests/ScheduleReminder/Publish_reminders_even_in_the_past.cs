﻿using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.Tests.ScheduleReminder
{
	[TestFixture ()]
	public class Publish_reminders_even_in_the_past : Scheduler_Spec
	{
		[TestFixtureSetUp]
		public void given_there_are_some_reminders_in_the_past()
		{
			SetNow(SystemTime.UtcNow());

			GivenA (new SystemMessage.Start ());

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now.AddMilliseconds (-100),
					"http://deliveryUrl",
					"content/type",
					new byte[0])));

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now.AddMilliseconds (-160),
					"http://deliveryUrl",
					"content/type",
					new byte[0])));

			GivenA (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					Now.AddMilliseconds (50),
					"http://deliveryUrl",
					"content/type",
					new byte[0])));
		}

		[Test]
		public void should_publish_all_past_reminders()
		{
			FireTimer ();
			Assert.AreEqual (2, Received.Count);

			AdvanceTimeBy (100);
			FireTimer ();
			Assert.AreEqual (3, Received.Count);
		}
	}
}

