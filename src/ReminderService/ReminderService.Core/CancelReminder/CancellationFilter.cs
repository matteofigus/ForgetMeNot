using System;
using System.Linq;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using log4net;

namespace ReminderService.Core
{
	public class CancellationFilter : 
		IConsume<ReminderMessage.Cancel>, 
		IConsume<ReminderMessage.Due>,
		IConsume<ClusterMessage.Replicate<ReminderMessage.Cancel>>
	{
		private readonly HashSet<ReminderMessage.Cancel> _cancellations;
		private readonly IConsume<ReminderMessage.Due> _innerHandler;
		private readonly ILog Logger = LogManager.GetLogger(typeof(CancellationFilter));

		public CancellationFilter (IConsume<ReminderMessage.Due> innerHandler)
		{
			var comparer = new ReminderMessage.EqualityComparer<ReminderMessage.Cancel> (
				               c => c.ReminderId.GetHashCode (),
				               (x, y) => x.ReminderId == y.ReminderId);
			_cancellations = new HashSet<ReminderMessage.Cancel> (comparer);

			Ensure.NotNull (innerHandler, "innerHandler");

			_innerHandler = innerHandler;
		}

		public void Handle(ClusterMessage.Replicate<ReminderMessage.Cancel> replicate)
		{
			Handle (replicate.InnerMessage);
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
				_innerHandler.Handle (due);
			else {
				_cancellations.Remove (found);
				Logger.Info (string.Format ("Cancelled Reminder [{0}] found and removed from cancellation list", due.ReminderId));
			}
		}
	}
}

