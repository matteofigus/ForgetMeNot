using System;
using ReminderService.Messages;
using ReminderService.Common;
using log4net;
using Npgsql;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core.Persistence.Postgres
{
	public class PostgresJournaler : IJournalEvents
	{
		private static readonly ILog Logger = LogManager.GetLogger (typeof(PostgresJournaler));
		private readonly ICommandFactory _commandFactory;
		private readonly string _connectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";

		public PostgresJournaler (ICommandFactory commandFactory, string connectionString)
		{
			Ensure.NotNull (commandFactory, "commandFactory");
			Ensure.NotNullOrEmpty (connectionString, "connectionString");

			_commandFactory = commandFactory;
			_connectionString = connectionString;
		}

		public void Write (IMessage message)
		{
			try {
				using (var connection = new NpgsqlConnection(_connectionString)) {
					using (var command = _commandFactory.BuildWriteCommand (message)) {
						command.Connection = connection;
						connection.Open();
						command.ExecuteNonQuery ();
					}
				}
			} 
			catch (Exception ex) {
				Logger.Error ("There was a problem while attempting to write the message to Postgres.", ex);
				throw;
			}
		}
	}
}

