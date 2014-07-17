using System;
using System.Collections.Generic;
using ReminderService.Router;
using ReminderService.Messages;


namespace ReminderService.Core.Tests.Helpers
{
	public abstract class RoutableBase
	{
		private readonly IBus _bus = new Bus ();
		private readonly List<IMessage> _received = new List<IMessage> ();

		public IBus Bus {
			get { return _bus; }
		}

		public void Subscribe<T>(IConsume<T> subscriber) where T : class, IMessage
		{
			_bus.Subscribe (subscriber);
		}

		public IList<IMessage> Received {
			get { return _received; }
		}

		public void ClearReceived()
		{
			_received.Clear ();
		}
	}
}

