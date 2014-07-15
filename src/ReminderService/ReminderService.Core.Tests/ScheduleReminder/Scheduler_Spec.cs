using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;

namespace ReminderService.Core.Tests
{
	[TestFixture]
	public abstract class Scheduler_Spec : IConsume<ReminderMessage.ScheduledReminderHasBeenJournaled>
	{
		private readonly TestTimer _timer = new TestTimer();
		private readonly IBus _bus = new Bus ();
		private readonly Scheduler _scheduler;
		private readonly List<IMessage> _receivedMessages = new List<IMessage>();

		public Scheduler_Spec ()
		{
			_scheduler  = new Scheduler (_bus, _timer);
			_bus.Subscribe (this);
		}

		public void GivenA(SystemMessage.Start startMessage)
		{
			_scheduler.Handle (startMessage);
		}

		public void GivenA(ReminderMessage.ScheduledReminderHasBeenJournaled journaledReminder)
		{
			_scheduler.Handle (journaledReminder);
		}

		public void Handle (ReminderMessage.ScheduledReminderHasBeenJournaled msg)
		{
			_receivedMessages.Add (msg);
		}

		public DateTime Now {
			get { return SystemTime.Now (); }
		}

		public void SetNow(DateTime now)
		{
			SystemTime.Set (now);
		}

		public void AdvanceTimeBy(int milliseconds)
		{
			SetNow(SystemTime.Now ().AddMilliseconds (milliseconds));
		}

		public void FireTimer()
		{
			_timer.Fire ();
		}

		public void FireTimer(DateTime atTime)
		{
			SystemTime.Set (atTime);
			_timer.Fire ();
		}

		public List<IMessage> Received
		{
			get { return _receivedMessages; }
		}
	}

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

