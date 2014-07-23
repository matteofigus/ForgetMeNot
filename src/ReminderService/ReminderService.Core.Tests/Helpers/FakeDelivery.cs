using System;
using ReminderService.Messages;

namespace ReminderService.Core.Tests.Helpers
{
	public class FakeDelivery : IDeliverReminders
	{
		private readonly Action<ReminderMessage.Due> _onSend;

		public FakeDelivery (Action<ReminderMessage.Due> onSend)
		{
			_onSend = onSend;
		}

		public void Send (ReminderMessage.Due reminder)
		{
			if (_onSend != null)
				_onSend (reminder);
		}
	}
}

