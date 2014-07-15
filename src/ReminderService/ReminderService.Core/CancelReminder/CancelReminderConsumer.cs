using System;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core
{
	public class CancelReminderConsumer : IConsume<ReminderMessage.CancelReminder>
	{
		public CancelReminderConsumer ()
		{

		}

		#region IConsume implementation

		public void Handle (ReminderMessage.CancelReminder msg)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

