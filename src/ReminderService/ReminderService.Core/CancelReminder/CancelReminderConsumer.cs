using System;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core
{
	public class CancelReminderConsumer : IConsume<ReminderMessages.CancelReminder>
	{
		public CancelReminderConsumer ()
		{

		}

		#region IConsume implementation

		public void Handle (ReminderMessages.CancelReminder msg)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

