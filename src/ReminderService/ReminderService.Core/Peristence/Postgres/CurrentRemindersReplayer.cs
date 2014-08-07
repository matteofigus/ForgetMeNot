using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.Persistence;
using ReminderService.Core.Persistence.Npgsql;
using System.Data;

namespace ReminderService.Core.Persistence.Postgres
{
	public class CurrentRemindersReplayer : IReplayEvents
	{
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
			return (IObservable<T>)_commandFactory.GetCurrentRemindersCommand ().ExecuteAsObservable (_reminderMapper);
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

