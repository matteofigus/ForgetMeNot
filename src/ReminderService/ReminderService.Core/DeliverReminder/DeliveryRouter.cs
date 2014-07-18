using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;
using OpenTable.Services.Components.Logging;

namespace ReminderService.Core.DeliverReminder
{
	public class DeliveryRouter : IConsume<ReminderMessage.DueReminderNotCanceled>
	{
		private readonly ILogger _logger;
		private readonly List<Func<ReminderMessage.DueReminderNotCanceled, bool>> _handlerChain;

		public DeliveryRouter (ILogger logger, IEnumerable<Func<ReminderMessage.DueReminderNotCanceled, bool>> handlers)
		{
			Ensure.NotNull (logger, "logger");
			Ensure.NotNull (handlers, "handlers");

			_logger = logger;
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
			_logger.LogException (Level.Error, exception, 
				string.Format("There are no reminder delivery handlers registered to deliver '{0}'", msg.DeliveryUrl));
			throw exception;
		}
	}
}

