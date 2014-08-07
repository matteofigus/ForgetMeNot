using System;
using System.Data;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.Persistence.Npgsql;
using Npgsql;

namespace ReminderService.Core.Persistence.Postgres
{
	public class CancellationReplayer : IReplayEvents
	{
		private readonly ICommandFactory _commandFactory;
		private readonly Func<IDataReader, ReminderMessage.Cancel> _cancellationMapper;
		private readonly string _connectionString;

		public CancellationReplayer (
			ICommandFactory commandFactory, 
			Func<IDataReader, ReminderMessage.Cancel> cancellationMapper = null)
		{
			Ensure.NotNull (commandFactory, "commandFactory");
			_commandFactory = commandFactory;

			if (cancellationMapper == null)
				_cancellationMapper = CancellationMap;
			else
				_cancellationMapper = cancellationMapper;
		}

		public IObservable<T> Replay<T> (DateTime from)
		{
			using (var connection = new NpgsqlConnection (_connectionString)) {
				using (var command = _commandFactory.GetCancellationsCommand (from)) {
					command.Connection = connection;
					connection.Open ();
					return (IObservable<T>)command.ExecuteAsObservable (_cancellationMapper);
				}
			}
		}

		public static Func<IDataReader, ReminderMessage.Cancel> CancellationMap {
			get { 
				return (reader) => new ReminderMessage.Cancel (reader.Get<Guid>("reminderId"));
			}
		}
	}
}

