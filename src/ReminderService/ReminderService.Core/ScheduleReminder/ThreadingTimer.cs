using System;
using System.Threading;

namespace ReminderService.Core.ScheduleReminder
{
	public class ThreadingTimer : ITimer
	{
		private Action _callback;
		private readonly Timer _timer;

		public ThreadingTimer()
		{
			_timer = new Timer(LapsedCallback, null, Timeout.Infinite, Timeout.Infinite);
		}

		private void LapsedCallback(object state)
		{
			if (_callback != null)
				_callback();
		}

		public void FiresIn(int milliseconds, Action callback)
		{
			_callback = callback;
			var dueTime = milliseconds == Timeout.Infinite ? Timeout.Infinite : Math.Max(0, milliseconds);
			_timer.Change(dueTime, Timeout.Infinite);
		}

		public void Dispose()
		{
			_timer.Dispose();
		}
	}
}

