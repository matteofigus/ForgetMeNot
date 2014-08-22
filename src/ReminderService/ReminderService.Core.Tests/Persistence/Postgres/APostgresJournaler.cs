using NUnit.Framework;
using System;
using System.Text;
using ReminderService.Core.Persistence.Postgres;
using ReminderService.Messages;
using ReminderService.Common;
using Npgsql;
using ReminderService.Test.Common;

namespace ReminderService.Core.Tests.Persistence.Postgres
{
	[TestFixture]
	public class APostgresJournaler : PostgresTestBase
	{
		private ICommandFactory _commandFactory;
		private PostgresJournaler _journaler;

		[TestFixtureSetUp]
		public void A_journaler()
		{
			_commandFactory = new PostgresCommandFactory ();
			_journaler = new PostgresJournaler (_commandFactory, ConnectionString);
		}

		[Test]
		public void Should_write_Schedule_messages ()
		{
			var reminderId = Guid.NewGuid ();
			var schedule = new ReminderMessage.Schedule (
				reminderId,
				SystemTime.Now(),
				"deliveryUrl",
				"application/json",
				Encoding.UTF8.GetBytes("{\"property1:\" \"value1\"}"));

			_journaler.Write (schedule);

			AssertReminderExists (reminderId);
		}

		[Test]
		public void Should_write_cancellations()
		{
			var reminderId = Guid.NewGuid ();
			var schedule = new ReminderMessage.Schedule (
				reminderId,
				SystemTime.Now(),
				"deliveryUrl",
				"application/json",
				Encoding.UTF8.GetBytes("{\"property1:\" \"value1\"}"));

			_journaler.Write (schedule);

			AssertReminderExists (reminderId);

			var cancellation = new ReminderMessage.Cancel (reminderId);

			_journaler.Write (cancellation);

			AssertCancelled (reminderId);
		}

		[Test]
		public void Should_write_Sent_messages()
		{
			var reminderId = Guid.NewGuid ();
			var schedule = new ReminderMessage.Schedule (
				reminderId,
				SystemTime.Now(),
				"deliveryUrl",
				"application/json",
				Encoding.UTF8.GetBytes("{\"property1:\" \"value1\"}"));

			_journaler.Write (schedule);

			AssertReminderExists (reminderId);

			var sent = new ReminderMessage.Delivered (reminderId, SystemTime.Now ());

			_journaler.Write (sent);

			AssertSent (reminderId);
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			CleanupDatabase ();
		}
	}
}

