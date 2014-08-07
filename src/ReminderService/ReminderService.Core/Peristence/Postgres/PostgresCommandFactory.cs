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
		const string GetCurrentReminders_CommandText = "SELECT * FROM public.reminders WHERE sent_time IS NULL AND cancelled = FALSE";
		const string GetCancellations_CommandText = "SELECT reminder_id FROM public.reminders WHERE cancelled = FALSE AND due_time > ";
		const string WriteCancellation_CommandText = "UPDATE public.reminders SET cancelled = TRUE WHERE reminder_id = '{0}'";
		const string WriteScheduleReminder_CommandText = "INSERT INTO public.reminders VALUES ('{0}', '{1}', '{2}', NULL, FALSE, 1, '{3}')";
		const string WriteSentReminder_CommandText = "UPDATE public.reminders SET sent_time = '{0}' WHERE reminder_id = '{1}'";
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
			return new NpgsqlCommand (GetCancellations_CommandText);
		}

		public IDbCommand GetCurrentRemindersCommand ()
		{
			return new NpgsqlCommand (GetCurrentReminders_CommandText);
		}

		public IDbCommand BuildWriteCommand<T> (T message) where T : IMessage
		{
			var command =  _commandSelector[message.GetType()](message);
			return command;
		}

		private Dictionary<Type, Func<IMessage, NpgsqlCommand>> WriteCommandSelector {
			get	{
				var dic = new Dictionary<Type, Func<IMessage, NpgsqlCommand>> ();
				dic.Add (typeof(ReminderMessage.Cancel), 	WriteCancellationCommand );
				dic.Add (typeof(ReminderMessage.Schedule), 	WriteScheduleCommand );
				dic.Add (typeof(ReminderMessage.Sent), 		WriteSentCommand );
				return dic;
			}
		}

		private Func<IMessage, NpgsqlCommand> WriteCancellationCommand {
			get {
				return (msg) => {
					var cancellation = msg as ReminderMessage.Cancel;
					return new NpgsqlCommand(string.Format(WriteCancellation_CommandText, cancellation.ReminderId));
				};
			}
		}

		private Func<IMessage, NpgsqlCommand> WriteScheduleCommand {
			get {
				return (message) => {
					var schedule = message as ReminderMessage.Schedule;
					return new NpgsqlCommand(
						string.Format(WriteScheduleReminder_CommandText, schedule.ReminderId, schedule.TimeoutAt, schedule.AsJson(), SystemTime.Now()));
				};
			}
		}

		private Func<IMessage, NpgsqlCommand> WriteSentCommand {
			get { 
				return (message) => {
					var sent = message as ReminderMessage.Sent;
					return new NpgsqlCommand(
						string.Format(WriteSentReminder_CommandText, sent.SentStamp, sent.ReminderId)
					);
				};
			}
		}
	}
}

