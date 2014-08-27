using System;
using ReminderService.Messages;

namespace ReminderService.Core
{
	public interface IDeliverReminders
	{
		//void Send(ReminderMessage.Due reminder);

		void Send (ReminderMessage.Schedule reminder, string url);
	}
}

