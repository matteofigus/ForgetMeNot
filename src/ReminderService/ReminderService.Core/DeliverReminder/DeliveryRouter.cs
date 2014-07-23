using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;
using log4net;

namespace ReminderService.Core.DeliverReminder
{
	public enum DeliveryTransport
	{
		None,
		HTTP,
		AMQP
	}

	public class DeliveryRouter : IConsume<ReminderMessage.Due>
	{
		private readonly ILog Logger = LogManager.GetLogger(typeof(DeliveryRouter));
		private readonly IDictionary<DeliveryTransport, IDeliverReminders> _handlers = new Dictionary<DeliveryTransport, IDeliverReminders> ();
		private readonly Func<ReminderMessage.Due, DeliveryTransport, bool> _handlerSelector = 
			(due, transport) => 
				transport == DeliveryTransport.HTTP &&
					due.DeliveryUrl.ToUpper ().StartsWith ("HTTP");

		public void AddHandler(DeliveryTransport transport, IDeliverReminders handler)
		{
			if(!_handlers.ContainsKey(transport))
				_handlers.Add(transport, handler);
		}

		public void Handle (ReminderMessage.Due msg)
		{
			foreach (var handler in _handlers) {
				if (_handlerSelector(msg, handler.Key)) {
					handler.Value.Send (msg);
					return;
				}
			}

			//if we get here then the transport scheme for the reminder is not supported.
			var exception = new NotSupportedException (string.Format("Delivery transport not supported for reminder [{0}]", msg.ReminderId));
			Logger.Error (
				string.Format("There are no reminder delivery handlers registered to deliver '{0}'", msg.DeliveryUrl), exception);
			throw exception;
		}
	}
}

