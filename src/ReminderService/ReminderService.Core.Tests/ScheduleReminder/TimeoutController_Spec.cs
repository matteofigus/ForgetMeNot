using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Common;
//using ReminderService.DataStructures.Tests.Helpers;

namespace ReminderService.Core.Tests.ScheduleReminder {

	[TestFixture]
	public class A_TimeoutController
	{
		private DateTime now = DateTime.Now;
		private TimeoutController timeoutController;
		private readonly List<ScheduledReminder> _pastReminders = 
			new List<ScheduledReminder> ();
		private ManualResetEvent _resetEvent = new ManualResetEvent (false);

		public A_TimeoutController ()
		{
			var reminderCallback = new Action<IEnumerable<ScheduledReminder>> ((r) => {
				_pastReminders.AddRange (r);
				Console.WriteLine(string.Format("{0} reminders due", r.Count()));
				if(_pastReminders.Count == 3)
					_resetEvent.Set();
			});
			ITimer timer = new ThreadingTimer ();
			timeoutController = new TimeoutController (timer,
				reminderCallback);
		}

		[TestFixtureSetUp] public void That_is_not_empty()
		{
			SystemTime.Set (now);
			timeoutController.Start ();
		}

		[Test] public void Pops_off_reminders_that_are_due()
		{
			Assert.AreEqual (0, _pastReminders.Count);
			SystemTime.Set (now.AddMilliseconds(-10));
			timeoutController.Add (new ScheduledReminder (
				Guid.NewGuid (),
				now,
				new object (), 
				new Uri ("http://some/place/3")));
			timeoutController.Add (new ScheduledReminder (
				Guid.NewGuid (),
				now.AddMilliseconds(200),
				new object (), 
				new Uri ("http://some/place/2")));
			timeoutController.Add (new ScheduledReminder (
				Guid.NewGuid (),
				now.AddSeconds(320),
				new object (), 
				new Uri ("http://some/place/1")));

			var signaled = _resetEvent.WaitOne (300);

			Assert.IsTrue (signaled);
			Assert.AreEqual (3, _pastReminders.Count);
			Assert.AreEqual (now, _pastReminders [0].TimeOutAt);
			Assert.AreEqual (new Uri ("http://some/place/3"), _pastReminders [0].Destination);

			Assert.AreEqual (now.AddMilliseconds(200), _pastReminders [1].TimeOutAt);
			Assert.AreEqual (new Uri ("http://some/place/2"), _pastReminders [1].Destination);

			Assert.AreEqual (now.AddSeconds(320), _pastReminders [2].TimeOutAt);
			Assert.AreEqual (new Uri ("http://some/place/1"), _pastReminders [2].Destination);
		}

		[Test] public void Adds_reminders_and_reprioritizes_existing()
		{

		}
	}
}

