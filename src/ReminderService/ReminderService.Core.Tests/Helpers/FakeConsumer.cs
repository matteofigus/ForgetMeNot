using System.Collections.Generic;
using ReminderService.Router;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core.Tests.Helpers
{
	public class FakeConsumer<T> : IConsume<T> where T : class, IMessage
	{
		private readonly List<IMessage> _received = new List<IMessage> ();
			
		public void Handle (T msg)
		{
			_received.Add (msg);
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

