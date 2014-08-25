﻿using NUnit.Framework;
using System;
using System.Linq;
using ReminderService.Core.DeliverReminder;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Test.Common;
using ReminderService.Common;
using ReminderService.Router.MessageInterfaces;
using System.IO;

namespace ReminderService.Core.Tests.DeliverReminder
{
	[TestFixture]
	public class When_attempting_redelivery : 
		RoutableTestBase, 
		IConsume<ReminderMessage.Schedule>,
		IConsume<ReminderMessage.Undeliverable>
	{

		private UndeliveredProcessManager _processManager;
		private ReminderMessage.Schedule _originalReminder;
		private TimeSpan _durationToGiveup = TimeSpan.FromMinutes(60);

		[TestFixtureSetUp]
		public void Initialize()
		{
			_processManager = new UndeliveredProcessManager (Bus);
			Subscribe<ReminderMessage.Schedule>(this);
			Subscribe<ReminderMessage.Undeliverable> (this);
			_originalReminder = MessageBuilders.BuildReminders (1, 3, SystemTime.UtcNow().Add(_durationToGiveup)).First ();
		}

		[Test]
		public void Run_Steps()
		{
			When_receive_an_undelivered_reminder (_originalReminder);
			while (!Undeliverable_message_received ()) {
				Should_emit_a_rescheduled_reminder_for_the_same_reminder (_originalReminder.ReminderId);
				When_receive_an_undelivered_reminder (_originalReminder);
			}

			//there should be 3 rescheduled reminders and one Undeliverable message
			Received.ContainsThisMany<ReminderMessage.Schedule>(3);
			Received.ContainsOne<ReminderMessage.Undeliverable> ();

			var timeSinceOriginalWasDue = ((ReminderMessage.Schedule)Received [Received.Count - 2]).RescheduleFor - _originalReminder.DueAt;

			//we hit some rounding errors in the math using DateTime's, so lets make sure we are close enough
			Assert.That (timeSinceOriginalWasDue, Is.EqualTo (_durationToGiveup).Within (1).Seconds);
		}

		public void When_receive_an_undelivered_reminder(ReminderMessage.Schedule reminder)
		{
			var undelivered = new ReminderMessage.Undelivered (reminder, "failed reason");
			_processManager.Handle (undelivered);
		}

		public void Should_emit_a_rescheduled_reminder_for_the_same_reminder(Guid reminderId)
		{
			var msg = Received[Received.Count -1];
			Assert.IsInstanceOf<ReminderMessage.Schedule> (msg);
			var received = (ReminderMessage.Schedule)msg;
			Assert.AreEqual (received.ReminderId, reminderId);
		}

		private bool Undeliverable_message_received()
		{
			if (Received.Count == 0)
				return false;
			return Received [Received.Count - 1] is ReminderMessage.Undeliverable;
		}

		public void Should_retry_until_GiveUp_is_reached(IMessage msg)
		{
			Assert.IsInstanceOf<ReminderMessage.Undeliverable> (msg);
		}

		public void Handle (ReminderMessage.Schedule msg)
		{
			Received.Add (msg);
		}

		public void Handle (ReminderMessage.Undeliverable msg)
		{
			Received.Add (msg);
		}
	}
}

