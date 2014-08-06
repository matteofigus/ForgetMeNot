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
		const string WriteCancellationText = "";
		const string WriteScheduleReminderText = "INSERT INTO public.reminders VALUES ('{0}', '{1}', '{2}', NULL, FALSE, 1, '{3}')";
		private readonly IDictionary<Type, Func<IMessage, NpgsqlCommand>> _commandSelector;

		public PostgresCommandFactory (
			IDictionary<Type, Func<IMessage, NpgsqlCommand>> commandSelector = null)
		{
			if (commandSelector == null)
				_commandSelector = WriteCommandSelector;
			else
				_commandSelector = commandSelector;
		}

		public IDbCommand GetCancellationsCommand (DateTime since)
		{
			return new NpgsqlCommand (GetCancellationsText);
		}

		public IDbCommand GetCurrentRemindersCommand ()
		{
			return new NpgsqlCommand (GetCurrentRemindersText);
		}

		public IDbCommand BuildWriteCommand<T> (T message) where T : IMessage
		{
			var command =  _commandSelector[message.GetType()](message);
			return command;
		}

		private Dictionary<Type, Func<IMessage, NpgsqlCommand>> WriteCommandSelector {
			get	{
				var dic = new Dictionary<Type, Func<IMessage, NpgsqlCommand>> ();
				dic.Add (typeof(ReminderMessage.Cancel), WriteCancellationCommand );
				dic.Add (typeof(ReminderMessage.Schedule), WriteScheduleCommand );
				return dic;
			}
		}

		private Func<IMessage, NpgsqlCommand> WriteCancellationCommand {
			get {
				return (cancel) => {
						return new NpgsqlCommand(WriteCancellationText);
				};
			}
		}

		private Func<IMessage, NpgsqlCommand> WriteScheduleCommand {
			get {
				return (message) => {
					var schedule = message as ReminderMessage.Schedule;
						return new NpgsqlCommand(
							string.Format(WriteScheduleReminderText, schedule.ReminderId, schedule.TimeoutAt, schedule.AsJson(), SystemTime.Now()));
				};
			}
		}
	}
}

