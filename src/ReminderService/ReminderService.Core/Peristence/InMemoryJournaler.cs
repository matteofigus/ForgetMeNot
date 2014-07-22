using System;
using ReminderService.Common;
using System.Collections.Concurrent;

namespace ReminderService.Core
{
	public class InMemoryJournaler : IJournaler
	{
		private readonly ConcurrentQueue<IMessage> _messages;

		public InMemoryJournaler ()
		{
			_messages = new ConcurrentQueue<IMessage> ();
		}
			
		public void Write (IMessage message)
		{
			_messages.Enqueue (message);
		}
	}
}

