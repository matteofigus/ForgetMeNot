using System;
using ReminderService.Messages;
using ReminderService.Common;
using log4net;

namespace ReminderService.Core
{
	public class PostgresJournaler : IJournalEvents
	{
		private static readonly ILog Logger = LogManager.GetLogger (typeof(PostgresJournaler));
		private readonly ICommandFactory _commandFactory;

		public PostgresJournaler (ICommandFactory commandFactory)
		{
			Ensure.NotNull (commandFactory, "commandFactory");
			_commandFactory = commandFactory;
		}

		public void Write (IMessage message)
		{
			try {
				var command = _commandFactory.BuildWriteCommand (message);
				command.ExecuteNonQuery ();
			} 
			catch (Exception ex) {
				Logger.Error ("There was a problem while attempting to write the message to Postgres.", ex);
				throw;
			}
		}
	}
}

