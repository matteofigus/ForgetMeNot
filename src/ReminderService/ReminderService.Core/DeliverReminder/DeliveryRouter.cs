using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;
using log4net;

namespace ReminderService.Core.DeliverReminder
{
	public class DeliveryRouter : IConsume<ReminderMessage.DueReminderNotCanceled>
	{
		private readonly ILog Logger = LogManager.GetLogger(typeof(DeliveryRouter));
		private readonly List<Func<ReminderMessage.DueReminderNotCanceled, bool>> _handlerChain;

		public DeliveryRouter (IEnumerable<Func<ReminderMessage.DueReminderNotCanceled, bool>> handlers)
		{
			Ensure.NotNull (handlers, "handlers");

			_handlerChain = new List<Func<ReminderMessage.DueReminderNotCanceled, bool>> (handlers);
		}

		public void Handle (ReminderMessage.DueReminderNotCanceled msg)
		{
			foreach (var handler in _handlerChain) {
				if (handler (msg))
					return;
			}

			//if we get here then the transport scheme for the reminder is not supported.
			var exception = new NotSupportedException (string.Format("Delivery scheme not supported for reminder [{0}]", msg.ReminderId));
			Logger.Error (
				string.Format("There are no reminder delivery handlers registered to deliver '{0}'", msg.DeliveryUrl), exception);
			throw exception;
		}
	}
}

