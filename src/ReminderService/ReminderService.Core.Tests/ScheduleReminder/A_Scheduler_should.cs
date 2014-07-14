﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Router;
using ReminderService.Messages;

namespace ReminderService.Core.Tests
{
	[TestFixture ()]
	public class A_Scheduler_should : IConsume<ReminderMessages.ScheduledReminderHasBeenJournaled>
	{
		private readonly TestTimer _timer = new TestTimer();
		private readonly IBus _bus = new Bus ();
		private Scheduler _scheduler;
		private readonly IList<IMessage> _receivedMessages = new List<IMessage>();

		[TestFixtureSetUp]
		public void Setup()
		{
			_scheduler  = new Scheduler (_bus, _timer);
			_bus.Subscribe (this);
			_scheduler.Start ();
		}
			
		[SetUp]
		public void BeforeEach()
		{
			_receivedMessages.Clear ();
			_scheduler.Start ();
		}

		public void Handle (ReminderMessages.ScheduledReminderHasBeenJournaled msg)
		{
			_receivedMessages.Add (msg);
		}

		[Test]
		public void not_do_anything_until_started()
		{
			_scheduler.Stop ();

			var reminder = new ReminderMessages.ScheduleReminder (
				"http://deliveryUrl",
				"content/type",
				SystemTime.Now (),
				new byte[0]);
			var journaledReminder = new ReminderMessages.ScheduledReminderHasBeenJournaled (reminder);
			_scheduler.Handle (journaledReminder);
			_timer.Fire ();

			Assert.AreEqual (0, _receivedMessages.Count);
		}

		[Test ()]
		public void publish_the_expected_type ()
		{
			var reminder = new ReminderMessages.ScheduleReminder (
				               "http://deliveryUrl",
				               "content/type",
				               SystemTime.Now (),
				               new byte[0]);
			var journaledReminder = new ReminderMessages.ScheduledReminderHasBeenJournaled (reminder);
			_scheduler.Handle (journaledReminder);
			_timer.Fire ();

			Assert.AreEqual (1, _receivedMessages.Count);
			Assert.IsInstanceOfType(typeof(ReminderMessages.ScheduledReminderHasBeenJournaled), _receivedMessages[0]);
			Assert.AreSame(journaledReminder, _receivedMessages[0]);
		}

		[Test]
		public void publish_reminders_as_they_become_due()
		{
			var now = SystemTime.Now ();
			SystemTime.Set (now);
			foreach (var reminder in LoadReminders()) {
				_scheduler.Handle (reminder);
			}

			_timer.Fire ();
			Assert.AreEqual (1, _receivedMessages.Count);

			SystemTime.Set (now.AddMilliseconds (17));
			_timer.Fire ();

			Assert.AreEqual (2, _receivedMessages.Count);

			SystemTime.Set (now.AddMilliseconds (25));
			_timer.Fire ();
		}

		private IEnumerable<ReminderMessages.ScheduledReminderHasBeenJournaled> LoadReminders()
		{
			var reminders = new List<ReminderMessages.ScheduledReminderHasBeenJournaled>();
			reminders.Add (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/1",
					"content/type",
					SystemTime.Now (),
					new byte[0])));

			reminders.Add (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/2",
					"content/type",
					SystemTime.Now ().AddMilliseconds(15),
					new byte[0])));


			reminders.Add (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/3",
					"content/type",
					SystemTime.Now ().AddMilliseconds(25),
					new byte[0])));

			reminders.Add (new ReminderMessages.ScheduledReminderHasBeenJournaled (
				new ReminderMessages.ScheduleReminder (
					"http://deliveryUrl/4",
					"content/type",
					SystemTime.Now ().AddMilliseconds(100),
					new byte[0])));

			return reminders;
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

