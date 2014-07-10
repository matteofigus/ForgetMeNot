using System;

namespace ReminderService.Core.ScheduleReminder
{
	public interface ITimer : IDisposable
	{
		void FiresIn (int milliseconds, Action callback);
	}
}

