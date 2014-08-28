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
		private readonly string _deadLetterUrl;
		private readonly ISendMessages _bus;
		private readonly IDictionary<DeliveryTransport, IDeliverReminders> _handlers = new Dictionary<DeliveryTransport, IDeliverReminders> ();
		private readonly Func<ReminderMessage.Schedule, DeliveryTransport, bool> _handlerSelector = 
			(due, transport) => 
				transport == DeliveryTransport.HTTP &&
					due.DeliveryUrl.ToUpper ().StartsWith ("HTTP");

		public DeliveryRouter (ISendMessages bus, string deadLetterUrl)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNullOrEmpty (deadLetterUrl, "deadLetterUrl");

			_bus = bus;
			_deadLetterUrl = deadLetterUrl;
		}

		public void AddHandler(DeliveryTransport transport, IDeliverReminders handler)
		{
			if(!_handlers.ContainsKey(transport))
				_handlers.Add(transport, handler);
		}

		public void Handle (ReminderMessage.Due due)
		{
			foreach (var handler in _handlers) {
				if (_handlerSelector(due.Reminder, handler.Key)) {
					handler.Value.Send (due.Reminder, due.Reminder.DeliveryUrl, OnSuccessfulDelivery, OnFailedDelivery);
					return;
				}
			}

			//if we get here then the transport scheme for the reminder is not supported.
			var exception = new NotSupportedException (string.Format("Delivery transport not supported for reminder [{0}]", due.ReminderId));
			Logger.Error (
				string.Format("There are no reminder delivery handlers registered to deliver '{0}'", due.Reminder.DeliveryUrl), exception);
			throw exception;
		}

		private void OnSuccessfulDelivery(ReminderMessage.Schedule sentReminder)
		{
			_bus.Send (new ReminderMessage.Delivered(sentReminder.ReminderId, SystemTime.UtcNow()));
		}

		private void OnFailedDelivery(ReminderMessage.Schedule failedReminder, string errorMessage)
		{
			_bus.Send (new ReminderMessage.Undelivered(failedReminder, errorMessage));
		}
	}
}

