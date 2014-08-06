using NUnit.Framework;
using System;
using System.Text;
using ReminderService.Core.Persistence.Postgres;
using ReminderService.Messages;
using ReminderService.Common;
using Npgsql;

namespace ReminderService.Core.Tests.Persistence.Postgres
{
	[TestFixture]
	public class APostgresJournaler
	{
		const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";
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
				"deliveryUrl",
				"deadletterUrl",
				"application/json",
				SystemTime.Now(),
				Encoding.UTF8.GetBytes("{\"property1:\" \"value1\"}"));

			_journaler.Write (schedule);
			AssertSingleReminder (reminderId);
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{
			CleanupDatabase ();
		}

		private void AssertSingleReminder(Guid reminderId)
		{
			var commandText = "SELECT count(*) FROM public.reminders WHERE reminder_id = '{0}'";
			using (var connection = new NpgsqlConnection (ConnectionString)) {
				using (var command = new NpgsqlCommand (string.Format (commandText, reminderId), connection)) {
					connection.Open ();
					var count = Convert.ToInt32( command.ExecuteScalar ());
					Assert.AreEqual (1, count);
				}
			}
		}

		private void CleanupDatabase()
		{
			var commandText = "DELETE FROM public.reminders";
			using (var connection = new NpgsqlConnection (ConnectionString)) {
				using (var command = new NpgsqlCommand (commandText, connection)) {
					connection.Open ();
					command.ExecuteNonQuery ();
				}
			}
		}
	}
}

