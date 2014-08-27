using System;
using ReminderService.Messages;

namespace ReminderService.Core.Tests.Helpers
{
	public class FakeDelivery : IDeliverReminders
	{
		private readonly Action<ReminderMessage.Schedule> _onSend;

		public FakeDelivery (Action<ReminderMessage.Schedule> onSend)
		{
			_onSend = onSend;
		}

		public void Send (ReminderMessage.Schedule reminder, string url)
		{
			if (_onSend != null)
				_onSend (reminder);
		}
	}
}

