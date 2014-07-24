using System;
using ReminderService.Core.ScheduleReminder;

namespace ReminderService.Test.Common
{
	public class TestTimer : ITimer
	{
		private Action _callback;

		public void FiresIn (int milliseconds, Action callback)
		{
			_callback = callback;
		}

		public void Fire()
		{
			if (_callback != null)
				_callback.Invoke ();
		}

		public void Dispose ()
		{
			//nothing to do...
		}
	}
}

