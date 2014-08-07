using NUnit.Framework;
using System;
using ReminderService.Core.Tests.Persistence.Postgres;
using ReminderService.Core.Persistence.Postgres;
using ReminderService.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReminderService.Common;
using ReminderService.Core.Persistence;
using System.Reactive;
using System.Reactive.Linq;

namespace ReminderService.Core.Tests.Persistence.Postgres
{
	[TestFixture ()]
	public class A_CancellationReplayer : PostgresTestBase
	{
		private DateTime _now;

		[TestFixtureSetUp]
		public void Given_there_are_current_reminders_in_the_database()
		{
			_now = SystemTime.Now ();
			var journaler = new PostgresJournaler (new PostgresCommandFactory(), ConnectionString);
			var reminders = BuildReminders (10);
			var cancellations = BuildCancellations (5, (IEnumerable<ReminderMessage.Schedule>)reminders);
			foreach (var item in reminders.Concat (cancellations).ToList()) {
				journaler.Write (item);
			}

			AssertNReminders (10);
		}

		[Test]
		public void Should_replay_all_cancellations()
		{
			var replayer = new CancellationReplayer(new PostgresCommandFactory());
			var observable = replayer.Replay<ReminderMessage.Cancel> (_now);
			Observable
				.Count (observable)
				.Subscribe (x => 
					Assert.AreEqual (5, x));
		}

		private IEnumerable<IMessage> BuildReminders(int count)
		{
			return Enumerable
				.Range (0, count)
				.Select (i => new ReminderMessage.Schedule (
					Guid.NewGuid(),
					"deliveryUrl",
					"deadletterUrl",
					"application/json",
					SystemTime.Now(),
					Encoding.UTF8.GetBytes("{\"property1:\" \"value1\"}")
				));
		}

		private IEnumerable<IMessage> BuildCancellations(int count, IEnumerable<ReminderMessage.Schedule> source)
		{
			return source
				.Select (r => new ReminderMessage.Cancel (r.ReminderId))
				.Take (count);
		}
	}
}

