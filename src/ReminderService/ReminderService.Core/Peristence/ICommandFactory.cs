using System;
using System.Data;
using ReminderService.Common;

namespace ReminderService.Core
{
	public interface ICommandFactory
	{
		IDbCommand GetCancellations (DateTime since);

		IDbCommand GetCurrentReminders ();

		IDbCommand BuildWriteCommand<TMessage> (TMessage message) where TMessage : IMessage;
	}
}

