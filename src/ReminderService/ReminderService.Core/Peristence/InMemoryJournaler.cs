using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core
{
	public class InMemoryJournaler : IJournalEvents
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

		public IList<IMessage> JournaledMessages {
			get { return _messages.ToList(); }
		}
	}
}

