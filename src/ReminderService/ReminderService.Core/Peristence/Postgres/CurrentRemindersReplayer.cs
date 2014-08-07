using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.Persistence;
using ReminderService.Core.Persistence.Npgsql;
using System.Data;
using Npgsql;
using Newtonsoft.Json;

namespace ReminderService.Core.Persistence.Postgres
{
	public class CurrentRemindersReplayer : IReplayEvents
	{
		private readonly string _connectionString;
		private readonly ICommandFactory _commandFactory;
		private readonly Func<IDataReader, ReminderMessage.Schedule> _reminderMapper;

		public CurrentRemindersReplayer (
			ICommandFactory commandFactory, 
			string connectionString,
			Func<IDataReader, ReminderMessage.Schedule> reminderMapper = null)
		{
			Ensure.NotNull (commandFactory, "commandFactory");
			Ensure.NotNullOrEmpty (connectionString, "connectionString");

			_commandFactory = commandFactory;
			_connectionString = connectionString;

			if (reminderMapper == null)
				_reminderMapper = ScheduleMap;
			else
				_reminderMapper = reminderMapper;
		}

		public IObservable<T> Replay<T> (DateTime from)
		{
			var connection = new NpgsqlConnection (_connectionString);
			var command = _commandFactory.GetCurrentRemindersCommand ();
			return (IObservable<T>)command.ExecuteAsObservable (connection, _reminderMapper);
		}

		public static Func<IDataReader, ReminderMessage.Schedule> ScheduleMap {
			get { 
				return (reader) => {
					var raw = reader["message"].ToString();
					return JsonConvert.DeserializeObject<ReminderMessage.Schedule>(raw);
				};
			}
		}
	}
}

