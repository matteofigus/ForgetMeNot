using System;

namespace ReminderService.Core.DeliverReminder
{
	public class AmqpDelivery : IDeliverReminders
	{
		public AmqpDelivery ()
		{
		}
			
		public void Send (ReminderService.Messages.ReminderMessage.Due reminder)
		{
			throw new NotImplementedException ();
		}
	}
}

