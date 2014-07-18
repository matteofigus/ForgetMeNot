using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Core;
using ReminderService.Router;
using ReminderService.Messages;
using OpenTable.Services.Components.Logging;
using ReminderService.Core.Tests.Helpers;

namespace ReminderService.Core.Tests
{
	[TestFixture]
	public class A_CancelledRemindersManager : RoutableTestBase, IConsume<ReminderMessage.DueReminderNotCanceled>
	{
		[SetUp]
		public void BeforeEach()
		{
			ClearReceived ();
		}

		[Test]
		public void should_keep_track_of_cancelled_reminders ()
		{
			var reminderId = Guid.NewGuid ();
			var due = new ReminderMessage.Due (reminderId,
				"delivery",
				"deadletter",
				"application/json",
				DateTime.Now,
				new byte[0]
			);

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle (due);

			Assert.AreEqual (0, Received.Count);
		}

		[Test]
		public void should_publish_reminders_that_have_not_been_cancelled()
		{
			var reminderId = Guid.NewGuid ();
			var cancelledReminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (cancelledReminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "deadletterurl","content", SystemTime.Now(), new byte[0]));

			Assert.AreEqual (1, Received.Count);
		}

		[Test]
		public void should_not_publish_cancelled_reminders()
		{
			var reminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "deadletterurl","content", SystemTime.Now(), new byte[0]));

			Assert.AreEqual (0, Received.Count);
		}

		[Test]
		public void should_not_keep_track_of_cancellations_once_they_are_due()
		{
			var reminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "deadletterurl","content", SystemTime.Now(), new byte[0]));
			//will handle this message the second time because it has been removed from the CancellationManagers internal list
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "deadletterurl","content", SystemTime.Now(), new byte[0]));

			Assert.AreEqual (1, Received.Count);
		}

		private readonly FakeLogger _logger;
		private readonly CancelledRemindersManager _cancellationManager;

		public A_CancelledRemindersManager ()
		{
			_logger = new FakeLogger ();
			_cancellationManager = new CancelledRemindersManager (Bus);
			Subscribe (this);
		}

		public void Handle (ReminderMessage.DueReminderNotCanceled msg)
		{
			Received.Add (msg);
		}
	}
}

