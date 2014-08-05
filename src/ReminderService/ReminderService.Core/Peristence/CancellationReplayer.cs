using System;
using System.Data;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.Persistence.Npgsql;

namespace ReminderService.Core.Persistence
{
	public class CancellationReplayer : IReplayEvents
	{
		private readonly ICommandFactory _commandFactory;
		private readonly Func<IDataReader, ReminderMessage.Cancel> _cancellationMapper;

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
			return (IObservable<T>)_commandFactory
				.GetCancellations (from)
				.ExecuteAsObservable (_cancellationMapper);
		}

		public static Func<IDataReader, ReminderMessage.Cancel> CancellationMap {
			get { 
				return (reader) => new ReminderMessage.Cancel (reader.Get<Guid>("reminderId"));
			}
		}
	}
}

