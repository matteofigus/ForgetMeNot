using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Core.Persistence.Postgres;
using System.Linq;
using ReminderService.Messages;
using System.Reactive.Linq;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Test.Common;

namespace ReminderService.Core.Tests.Persistence.Postgres
{
	[TestFixture ()]
	public class A_CurrentReminderReplayer : PostgresTestBase
	{
		private DateTime _now;

		[TestFixtureSetUp]
		public void Given_there_are_current_reminders_in_the_database()
		{
			CleanupDatabase ();
			_now = SystemTime.Now ();
			var journaler = new PostgresJournaler (new PostgresCommandFactory(), ConnectionString);
			var reminders = MessageBuilders.BuildReminders (10).ToList();
			foreach (var reminder in reminders) {
				journaler.Write (reminder);
			}
			var cancellations = MessageBuilders.BuildCancellationsAsSubsetOfReminders (5, reminders);
			foreach (var cancel in cancellations) {
				journaler.Write (cancel);
			}

			AssertNReminders (10);
		}

		[Test]
		public void Should_replay_all_current_reminders()
		{
			var replayer = new CurrentRemindersReplayer(new PostgresCommandFactory(), ConnectionString);
			var observable = replayer.Replay<JournaledEnvelope<ReminderMessage.Schedule>> (_now);
			Observable
				.Count (observable)
				.Subscribe (x => 
					Assert.AreEqual (5, x));
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			CleanupDatabase ();
		}
	}
}

