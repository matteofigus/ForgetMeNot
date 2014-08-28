using System;
using System.Data;
using Npgsql;
using ReminderService.Messages;
using System.Collections.Generic;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Core.Persistence.Postgres
{
	public class PostgresCommandFactory : ICommandFactory
	{
		const string GetCurrentReminders_CommandText = "SELECT * FROM public.reminders WHERE sent_time IS NULL AND cancelled = FALSE AND undelivered = FALSE AND undeliverable = FALSE";
		const string GetCancellations_CommandText = "SELECT reminder_id FROM public.reminders WHERE cancelled = TRUE AND due_time >= '{0}'";
		const string GetUndeliveredReminders_CommandText = "SELECT * FROM public.reminders WHERE undelivered = TRUE AND undeliverable = FALSE";
		const string WriteCancellation_CommandText = "UPDATE public.reminders SET cancelled = TRUE WHERE reminder_id = '{0}'";
		const string WriteScheduleReminder_CommandText = "INSERT INTO public.reminders VALUES ('{0}', '{1}', '{2}', NULL, FALSE, NULL, FALSE, FALSE, 1, '{3}')";
		const string WriteSentReminder_CommandText = "UPDATE public.reminders SET sent_time = '{0}' WHERE reminder_id = '{1}'";
		const string WriteUndeliverable_CommandText = "UPDATE public.reminders SET undeliverable = TRUE WHERE reminder_id = '{0}'";
		const string WriteUndelivered_CommandText = "UPDATE public.reminders SET undelivered = TRUE, undelivered_reason = '{1}' WHERE reminder_id = '{0}'";

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
			var command = new NpgsqlCommand (string.Format(GetCancellations_CommandText, since));
			command.CommandType = CommandType.Text;
			return command;
		}

		public IDbCommand GetCurrentRemindersCommand ()
		{
			return new NpgsqlCommand (GetCurrentReminders_CommandText);
		}

		public IDbCommand GetUndeliveredRemindersCommand ()
		{
			return new NpgsqlCommand (GetUndeliveredReminders_CommandText);
		}

		public IDbCommand BuildWriteCommand<T> (T message) where T : IMessage
		{
			var command =  _commandSelector[message.GetType()](message);
			return command;
		}

		private Dictionary<Type, Func<IMessage, NpgsqlCommand>> WriteCommandSelector {
			get	{
				var dic = new Dictionary<Type, Func<IMessage, NpgsqlCommand>> ();
				dic.Add (typeof(ReminderMessage.Cancel), 			WriteCancellationCommand );
				dic.Add (typeof(ReminderMessage.Schedule), 			WriteScheduleCommand );
				dic.Add (typeof(ReminderMessage.Delivered), 		WriteSentCommand );
				dic.Add (typeof(ReminderMessage.Undeliverable),		WriteUndeliverableCommand);
				dic.Add (typeof(ReminderMessage.Undelivered),		WriteUndeliveredCommand);
				dic.Add (typeof(ReminderMessage.SentToDeadLetter),	WriteSentToDeadLetterCommand);
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
						string.Format(WriteScheduleReminder_CommandText, schedule.ReminderId, schedule.DueAt, schedule.AsJson(), SystemTime.Now()));
				};
			}
		}

		public Func<IMessage, NpgsqlCommand> WriteSentCommand {
			get { 
				return (message) => {
					var sent = message as ReminderMessage.Delivered;
					return new NpgsqlCommand(
						string.Format(WriteSentReminder_CommandText, sent.SentStamp, sent.ReminderId)
					);
				};
			}
		}

		public Func<IMessage, NpgsqlCommand> WriteSentToDeadLetterCommand {
			get { 
				return (message) => {
					var sent = message as ReminderMessage.SentToDeadLetter;
					return new NpgsqlCommand(
						string.Format(WriteSentReminder_CommandText, sent.SentStamp, sent.ReminderId)
					);
				};
			}
		}

		public Func<IMessage, NpgsqlCommand> WriteUndeliverableCommand {
			get {
				return (message) => {
					var undeliverable = message as ReminderMessage.Undeliverable;
					return new NpgsqlCommand(
						string.Format(WriteUndeliverable_CommandText, undeliverable.ReminderId, undeliverable.Reason)
					);
				};
			}
		}

		public Func<IMessage, NpgsqlCommand> WriteUndeliveredCommand {
			get {
				return (message) => {
					var undelivered = message as ReminderMessage.Undelivered;
					return new NpgsqlCommand(
						string.Format(WriteUndelivered_CommandText, undelivered.ReminderId, undelivered.Reason)
					);
				};
			}
		}
	}
}

