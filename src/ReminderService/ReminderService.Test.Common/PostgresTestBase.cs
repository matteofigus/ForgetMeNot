using System;
using System.Data;
using Npgsql;
using NUnit.Framework;

namespace ReminderService.Test.Common
{
	public abstract class PostgresTestBase
	{
		protected const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";

		protected void AssertReminderExists(Guid reminderId)
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

		protected void AssertNReminders(int N)
		{
			var commandText = "SELECT count(*) FROM public.reminders";
			using (var connection = new NpgsqlConnection (ConnectionString)) {
				using (var command = new NpgsqlCommand (commandText, connection)) {
					connection.Open ();
					var count = Convert.ToInt32( command.ExecuteScalar ());
					Assert.AreEqual (N, count);
				}
			}
		}

		protected void AssertCancelled(Guid reminderId)
		{
			var commandText = 
				string.Format("SELECT count(*) FROM public.reminders WHERE reminder_id = '{0}' and cancelled = TRUE",
					reminderId);
			using (var connection = new NpgsqlConnection (ConnectionString)) {
				using (var command = new NpgsqlCommand (commandText, connection)) {
					connection.Open ();
					var count = Convert.ToInt32( command.ExecuteScalar ());
					Assert.AreEqual (1, count);
				}
			}
		}

		protected void AssertSent(Guid reminderId)
		{
			var commandText = 
				string.Format("SELECT count(*) FROM public.reminders WHERE reminder_id = '{0}' and cancelled = FALSE AND sent_time IS NOT NULL",
					reminderId);
			using (var connection = new NpgsqlConnection (ConnectionString)) {
				using (var command = new NpgsqlCommand (commandText, connection)) {
					connection.Open ();
					var count = Convert.ToInt32( command.ExecuteScalar ());
					Assert.AreEqual (1, count);
				}
			}
		}

		protected void CleanupDatabase()
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

