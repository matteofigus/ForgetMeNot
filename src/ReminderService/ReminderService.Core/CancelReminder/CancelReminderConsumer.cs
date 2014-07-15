using System;
using System.Linq;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using OpenTable.Services.Components.Logging;

namespace ReminderService.Core
{
	public class CancelReminderConsumer : IConsume<ReminderMessage.CancelReminder>, IConsume<ReminderMessage.DueReminder>
	{
		private readonly HashSet<ReminderMessage.CancelReminder> _cancellations;
		private readonly IBus _bus;
		private readonly ILogger _logger;

		public CancelReminderConsumer (IBus bus, ILogger logger)
		{
			var comparer = new ReminderMessage.EqualityComparer<ReminderMessage.CancelReminder> (
				               c => c.ReminderId.GetHashCode (),
				               (x, y) => x.ReminderId == y.ReminderId);
			_cancellations = new HashSet<ReminderMessage.CancelReminder> (comparer);
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (logger, "logger");

			_bus = bus;
			_logger = logger;
		}

		public void Handle (ReminderMessage.CancelReminder msg)
		{
			if (!_cancellations.Any (x => x.ReminderId == msg.ReminderId)) {
				_cancellations.Add (msg);
				_logger.Log (Level.Info, string.Format("Cancellation for reminder [{0}] added to cancellation list", msg.ReminderId));
			}
		}

		public void Handle (ReminderMessage.DueReminder due)
		{
			var found = _cancellations.SingleOrDefault (x => x.ReminderId == due.ReminderId);

			if (found == null)
				_bus.Publish (ReminderMessage.DueReminderNotCanceled.CreateFrom (due));
			else
				_cancellations.Remove (found);
				_logger.Log (Level.Info, string.Format("Cancelled Reminder [{0}] found and removed from cancellation list", due.ReminderId));
		}
	}
}

