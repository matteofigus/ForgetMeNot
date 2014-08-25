using NUnit.Framework;
using System;
using System.Linq;
using ReminderService.Core.DeliverReminder;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Test.Common;
using ReminderService.Common;

namespace ReminderService.Core.Tests.DeliverReminder
{
	[TestFixture]
	public class An_UndeliveredProcessManager : 
		RoutableTestBase, 
		IConsume<ReminderMessage.Schedule>,
		IConsume<ReminderMessage.Undeliverable>
	{
		private UndeliveredProcessManager _processManager;
		private ReminderMessage.Schedule _originalReminder;

		[TestFixtureSetUp]
		public void Initialize()
		{
			_processManager = new UndeliveredProcessManager (Bus);
			Subscribe<ReminderMessage.Schedule>(this);
			Subscribe<ReminderMessage.Undeliverable> (this);
			When_receive_an_undelivered_reminder ();
		}

		public void When_receive_an_undelivered_reminder()
		{
			_originalReminder = MessageBuilders.BuildReminders (1, 150, SystemTime.UtcNow().AddMilliseconds(1000)).First ();
			var undelivered = new ReminderMessage.Undelivered (_originalReminder, "failed reason");
			_processManager.Handle (undelivered);
		}

		[Test]
		public void Should_emit_a_rescheduled_reminder_for_the_same_reminder()
		{
			Assert.AreEqual (1, Received.Count);
			Assert.IsInstanceOf<ReminderMessage.Schedule> (Received.First());
			var received = (ReminderMessage.Schedule)Received.First ();
			Assert.AreEqual (received.ReminderId, _originalReminder.ReminderId);
		}

		[Test]
		public void Should_increase_the_Reschedule_time()
		{
			Assert.AreEqual (1, Received.Count);
			Assert.IsInstanceOf<ReminderMessage.Schedule> (Received.First());
			var received = (ReminderMessage.Schedule)Received.First ();
			Assert.AreEqual (_originalReminder.DueAt.AddMilliseconds(_originalReminder.FirstWaitDurationMs), received.RescheduleFor);
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

