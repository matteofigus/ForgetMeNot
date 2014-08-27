using System;
using NUnit.Framework;
using ReminderService.Test.Common;
using ReminderService.Core.Persistence.Postgres;
using ReminderService.Common;
using System.Linq;
using ReminderService.Messages;
using System.Reactive.Linq;

namespace ReminderService.Core.Tests.Persistence.Postgres
{
	[TestFixture]
	public class A_UndeliveredReminderReplayer : PostgresTestBase
	{
		[TestFixtureSetUp]
		public void Given_there_are_current_reminders_in_the_database()
		{
			CleanupDatabase ();

			var journaler = new PostgresJournaler (new PostgresCommandFactory(), ConnectionString);

			//post some reminders
			var reminders = MessageBuilders.BuildReminders (10).ToList();
			var undelivered = reminders.Take (3).Select (r => new ReminderMessage.Undelivered(r, "undelivered reason"));

			foreach (var reminder in reminders) {
				journaler.Write (reminder);
			}

			//post some of those reminders as undelivered
			foreach (var reminder in undelivered) {
				journaler.Write (reminder);
			}

			AssertNReminders (10);
		}

		[Test]
		public void Should_replay_all_undelivered_reminders()
		{
			var replayer = new UndeliveredRemindersReplayer(new PostgresCommandFactory(), ConnectionString);
			var observable = replayer.Replay<ReminderMessage.Undelivered> (SystemTime.Now());
			Observable
				.Count (observable)
				.Subscribe (x => 
					Assert.AreEqual (3, x));
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			CleanupDatabase ();
		}
	}
}

