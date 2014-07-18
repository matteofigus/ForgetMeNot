using System;
using ReminderService.Common;

namespace ReminderService.Core
{
	public interface IJournaler
	{
		void Write(IMessage message);
	}
}

