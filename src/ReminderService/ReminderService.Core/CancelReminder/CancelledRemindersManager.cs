using System;
using System.Linq;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using log4net;

namespace ReminderService.Core
{
	public class CancelledRemindersManager : 
		IConsume<ReminderMessage.Cancel>, 
		IConsume<ReminderMessage.Due>
	{
		private readonly HashSet<ReminderMessage.Cancel> _cancellations;
		private readonly IBus _bus;
		private readonly ILog Logger = LogManager.GetLogger(typeof(CancelledRemindersManager));

		public CancelledRemindersManager (IBus bus)
		{
			var comparer = new ReminderMessage.EqualityComparer<ReminderMessage.Cancel> (
				               c => c.ReminderId.GetHashCode (),
				               (x, y) => x.ReminderId == y.ReminderId);
			_cancellations = new HashSet<ReminderMessage.Cancel> (comparer);
			Ensure.NotNull (bus, "bus");

			_bus = bus;
		}

		public void Handle (ReminderMessage.Cancel msg)
		{
			if (!_cancellations.Any (x => x.ReminderId == msg.ReminderId)) {
				_cancellations.Add (msg);
				Logger.Info (string.Format("Cancellation for reminder [{0}] added to cancellation list", msg.ReminderId));
			}
		}

		public void Handle (ReminderMessage.Due due)
		{
			var found = _cancellations.SingleOrDefault (x => x.ReminderId == due.ReminderId);

			if (found == null)
				_bus.Publish (ReminderMessage.DueReminderNotCanceled.CreateFrom (due));
			else
				_cancellations.Remove (found);
				Logger.Info (string.Format("Cancelled Reminder [{0}] found and removed from cancellation list", due.ReminderId));
		}
	}
}

