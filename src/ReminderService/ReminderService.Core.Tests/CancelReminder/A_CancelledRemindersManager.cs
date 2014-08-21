using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Core;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Test.Common;

namespace ReminderService.Core.Tests.CancelReminder
{
	[TestFixture]
	public class A_CancelledRemindersManager
	{
		[SetUp]
		public void BeforeEach()
		{
			_fakeConsumer.ClearReceived ();
		}

		[Test]
		public void should_keep_track_of_cancelled_reminders ()
		{
			var reminderId = Guid.NewGuid ();
			var due = new ReminderMessage.Due (reminderId,
				"delivery",
				"application/json",
				DateTime.Now,
				new byte[0]
			);

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle (due);

			//since the reminder has been cancelled, then the Due message will get blocked by the CancellationManager
			_fakeConsumer.Received.DoesNotContainAnyThing ();
		}

		[Test]
		public void should_publish_reminders_that_have_not_been_cancelled()
		{
			var reminderId = Guid.NewGuid ();
			var cancelledReminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (cancelledReminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl","content", SystemTime.Now(), new byte[0]));

			_fakeConsumer.Received.ContainsOne<ReminderMessage.Due>();
		}

		[Test]
		public void should_not_publish_cancelled_reminders()
		{
			var reminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl","content", SystemTime.Now(), new byte[0]));

			_fakeConsumer.Received.DoesNotContainAnyThing ();
		}

		[Test]
		public void should_not_keep_track_of_cancellations_once_they_are_due()
		{
			var reminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl","content", SystemTime.Now(), new byte[0]));
			//will handle this message the second time because it has been removed from the CancellationManagers internal list
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl","content", SystemTime.Now(), new byte[0]));

			_fakeConsumer.Received.ContainsOne<ReminderMessage.Due>();
		}

		private readonly FakeConsumer<ReminderMessage.Due> _fakeConsumer;
		private readonly CancellationFilter _cancellationManager;

		public A_CancelledRemindersManager ()
		{
			_fakeConsumer = new FakeConsumer<ReminderMessage.Due> ();
			_cancellationManager = new CancellationFilter (_fakeConsumer);
		}
	}
}

