using NUnit.Framework;
using System;
using ReminderService.Core.Tests.Persistence.Postgres;
using ReminderService.Core.Persistence.Postgres;
using ReminderService.Messages;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Core.Persistence;
using System.Reactive;
using System.Reactive.Linq;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Test.Common;

namespace ReminderService.Core.Tests.Persistence.Postgres
{
	[TestFixture ()]
	public class A_CancellationReplayer : PostgresTestBase
	{
		private DateTime _now;

		[TestFixtureSetUp]
		public void Given_there_are_current_reminders_in_the_database()
		{
			CleanupDatabase ();
			_now = SystemTime.Now ();
			var journaler = new PostgresJournaler (new PostgresCommandFactory(), ConnectionString);
			var reminders = MessageBuilders.BuildReminders (10);
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
		public void Should_replay_cancellations()
		{
			var replayer = new CancellationReplayer(new PostgresCommandFactory(), ConnectionString);
			var observable = replayer.Replay<ReminderMessage.Cancel> (_now);
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

