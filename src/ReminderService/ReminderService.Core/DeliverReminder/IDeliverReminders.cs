using System;
using ReminderService.Messages;

namespace ReminderService.Core
{
	public interface IDeliverReminders
	{
		void Send (ReminderMessage.Schedule dueReminder, string url, Action<ReminderMessage.Schedule> onSuccessfulSend, Action<ReminderMessage.Schedule, string> onFailedSend);
	}
}

