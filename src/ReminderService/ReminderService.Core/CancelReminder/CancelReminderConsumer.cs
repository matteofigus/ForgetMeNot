using System;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core
{
	public class CancelReminderConsumer : IConsume<CancelReminder>
	{
		public CancelReminderConsumer ()
		{

		}

		#region IConsume implementation

		public void Handle (CancelReminder msg)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}
}

