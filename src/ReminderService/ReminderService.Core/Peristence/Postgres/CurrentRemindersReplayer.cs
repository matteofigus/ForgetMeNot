using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.Persistence;
using ReminderService.Core.Persistence.Npgsql;
using System.Data;
using Npgsql;

namespace ReminderService.Core.Persistence.Postgres
{
	public class CurrentRemindersReplayer : IReplayEvents
	{
		private readonly string _connectionString;
		private readonly ICommandFactory _commandFactory;
		private readonly Func<IDataReader, ReminderMessage.Schedule> _reminderMapper;

		public CurrentRemindersReplayer (
			ICommandFactory commandFactory, 
			Func<IDataReader, ReminderMessage.Schedule> reminderMapper = null)
		{
			Ensure.NotNull (commandFactory, "commandFactory");
			_commandFactory = commandFactory;


			_reminderMapper = reminderMapper;
		}

		public IObservable<T> Replay<T> (DateTime from)
		{
			using (var connection = new NpgsqlConnection(_connectionString)) {
				using (var command = _commandFactory.GetCurrentRemindersCommand ()) {
					//return (IObservable<T>)_commandFactory
					//.GetCurrentRemindersCommand ()
					command.Connection = connection;
					return (IObservable<T>)command.ExecuteAsObservable (connection, _reminderMapper);
				}
			}
		}

		public static Func<IDataReader, ReminderMessage.Schedule> ScheduleMap {
			get { 
				return (reader) =>
					new ReminderMessage.Schedule (
					reader.Get<string> ("deliveryUrl"),
					reader.Get<string> ("deadletterUrl"),
					reader.Get<string> ("contentType"),
					reader.Get<DateTime> ("timeoutAt"),
					reader.Get<byte[]> ("payload")
				);
			}
		}
	}
}

