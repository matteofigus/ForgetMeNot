using System;
using ReminderService.Common;

namespace ReminderService.Core
{
	public interface IJournalEvents
	{
		void Write(IMessage message);
	}
}

