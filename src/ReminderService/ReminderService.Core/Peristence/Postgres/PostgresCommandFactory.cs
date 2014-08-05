using System;
using System.Data;
using Npgsql;
using ReminderService.Common;
using ReminderService.Messages;
using System.Collections.Generic;

namespace ReminderService.Core.Persistence.Postgres
{
	public class PostgresCommandFactory : ICommandFactory
	{
		const string GetCurrentRemindersText = "SELECT * FROM public.reminders WHERE sent_time IS NULL AND cancelled = FALSE";
		const string GetCancellationsText = "SELECT reminder_id FROM public.reminders WHERE cancelled = FALSE AND due_time > ";
		private readonly string _connectionString;
		private readonly IDictionary<Type, Func<IMessage, NpgsqlCommand>> _commandSelector;

		public PostgresCommandFactory (
			string connectionString, 
			IDictionary<Type, Func<IMessage, NpgsqlCommand>> commandSelector = null)
		{
			Ensure.NotNullOrEmpty (connectionString, "connectionString");
			_connectionString = connectionString;

			if (commandSelector == null)
				_commandSelector = WriteCommandSelector;
			else
				_commandSelector = commandSelector;
		}

		public IDbCommand GetCancellationsCommand (DateTime since)
		{
			using (var connection = new NpgsqlConnection(_connectionString)) {
				connection.Open ();
				return new NpgsqlCommand (GetCancellationsText, connection);
			}
		}

		public IDbCommand GetCurrentRemindersCommand ()
		{
			using (var connection = new NpgsqlConnection(_connectionString)) {
				connection.Open ();
				return new NpgsqlCommand (GetCurrentRemindersText, connection);
			}
		}

		public IDbCommand BuildWriteCommand<T> (T message) where T : IMessage
		{
			using (var connection = new NpgsqlConnection (_connectionString)) {
				var command =  _commandSelector[typeof(T)](message);
				command.Connection = connection;
				connection.Open ();
				return command;
			}
		}

		public static Dictionary<Type, Func<IMessage, NpgsqlCommand>> WriteCommandSelector {
			get	{
				var dic = new Dictionary<Type, Func<IMessage, NpgsqlCommand>> ();
				dic.Add (typeof(ReminderMessage.Cancel), CancellationCommand );
				dic.Add (typeof(ReminderMessage.Schedule), ScheduleCommand );
				return dic;
			}
		}

		public static Func<IMessage, NpgsqlCommand> CancellationCommand {
			get {
				return (cancel) => {
					return new NpgsqlCommand();
				};
			}
		}

		public static Func<IMessage, NpgsqlCommand> ScheduleCommand {
			get {
				return (schedule) => {
					return new NpgsqlCommand();
				};
			}
		}
	}
}

