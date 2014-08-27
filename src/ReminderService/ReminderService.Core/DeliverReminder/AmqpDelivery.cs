using System;
using ReminderService.Messages;

namespace ReminderService.Core.DeliverReminder
{
	public class AmqpDelivery : IDeliverReminders
	{
		public AmqpDelivery ()
		{
		}
			
		public void Send (ReminderMessage.Schedule reminder, string url)
		{
			throw new NotImplementedException ();
		}
	}
}

