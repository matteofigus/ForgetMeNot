using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Test.Common;

namespace ReminderService.Core.Tests.ScheduleReminder
{
	[TestFixture]
	public abstract class Scheduler_Spec : RoutableTestBase, IConsume<ReminderMessage.Due>
	{
		private readonly TestTimer _timer = new TestTimer();
		private readonly Scheduler _scheduler;

		public Scheduler_Spec ()
		{
			_scheduler  = new Scheduler (Bus, _timer);
			Subscribe (this);
		}

		public void GivenA(SystemMessage.Start startMessage)
		{
			_scheduler.Handle (startMessage);
		}

		public void GivenA(Envelopes.Journaled<ReminderMessage.Schedule> journaledReminder)
		{
			_scheduler.Handle (journaledReminder);
		}

		public void Handle (ReminderMessage.Due msg)
		{
			Received.Add (msg);
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
	}
}

