using System;
using ReminderService.Messages;

namespace ReminderService.Core.DeliverReminder
{
	public class AmqpDelivery : IDeliverReminders
	{
		public AmqpDelivery ()
		{
		}

		public void Send (ReminderMessage.Schedule dueReminder, string url, Action<ReminderMessage.Schedule> onSuccessfulSend, Action<ReminderMessage.Schedule, string> onFailedSend)
		{
			throw new NotImplementedException ();
		}
	}
}

