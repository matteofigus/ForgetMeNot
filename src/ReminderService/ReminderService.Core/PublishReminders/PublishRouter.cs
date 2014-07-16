using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;

namespace ReminderService.Core
{
	public class PublishRouter : IConsume<ReminderMessage.DueReminderNotCanceled>
	{
		//private readonly ILog _logger;
		private readonly IBus _bus;
		private readonly List<Func<ReminderMessage.DueReminderNotCanceled, bool>> _handlerChain;

		public PublishRouter (IBus bus, IEnumerable<Func<ReminderMessage.DueReminderNotCanceled, bool>> handlers)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (handlers, "handlers");

			_bus = bus;
			_handlerChain = new List<Func<ReminderMessage.DueReminderNotCanceled, bool>> (handlers);
		}

		public void Handle (ReminderMessage.DueReminderNotCanceled msg)
		{
			foreach (var handler in _handlerChain) {
				if (handler (msg))
					return;
			}

			//if we get here then the transport scheme for the reminder is not supported.
			throw new NotSupportedException (string.Format("Delivery scheme not supported for reminder [{0}]", msg.ReminderId));
		}
	}
}

