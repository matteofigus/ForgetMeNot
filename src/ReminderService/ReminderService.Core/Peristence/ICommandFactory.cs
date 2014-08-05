using System;
using System.Data;
using ReminderService.Common;

namespace ReminderService.Core
{
	public interface ICommandFactory
	{
		IDbCommand GetCancellationsCommand (DateTime since);

		IDbCommand GetCurrentRemindersCommand ();

		IDbCommand BuildWriteCommand<TMessage> (TMessage message) where TMessage : IMessage;
	}
}

