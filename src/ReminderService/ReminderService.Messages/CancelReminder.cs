using System;
using ReminderService.Common;
using ReminderService.Router;

namespace ReminderService.Messages
{
	public class CancelReminder : IMessage
	{
		public Guid ReminderId { get; set; }

		public CancelReminder (Guid reminderId)
		{
			ReminderId = reminderId;
		}
	}
}

