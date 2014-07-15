using System;
using ReminderService.Router;

namespace ReminderService.Core
{
	public interface IJournaler
	{
		void Write(IMessage message);
	}
}

