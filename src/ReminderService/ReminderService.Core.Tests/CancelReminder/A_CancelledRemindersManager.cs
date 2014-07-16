using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Core;
using ReminderService.Router;
using ReminderService.Messages;
using OpenTable.Services.Components.Logging;

namespace ReminderService.Core.Tests
{
	[TestFixture]
	public class A_CancelledRemindersManager : IConsume<ReminderMessage.DueReminderNotCanceled>
	{
		private readonly IBus _bus = new Bus ();
		private readonly FakeLogger _logger;
		private readonly CancelledRemindersManager _cancellationManager;
		private readonly List<IMessage> _received = new List<IMessage> ();

		public A_CancelledRemindersManager ()
		{
			_logger = new FakeLogger ();
			_cancellationManager = new CancelledRemindersManager (_bus, _logger);
			_bus.Subscribe (this);
		}

		public void Handle (ReminderMessage.DueReminderNotCanceled msg)
		{
			_received.Add (msg);
		}

		[SetUp]
		public void BeforeEach()
		{
			_received.Clear ();
		}

		[Test]
		public void should_keep_track_of_cancelled_reminders ()
		{
			var reminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));

			Assert.IsTrue(
				_logger.LastLoggedMessage.StartsWith(
					string.Format("Cancellation for reminder [{0}] added to cancellation list", reminderId)
				));
		}

		[Test]
		public void should_publish_reminders_that_have_not_been_cancelled()
		{
			var reminderId = Guid.NewGuid ();
			var cancelledReminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (cancelledReminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "content", SystemTime.Now(), new byte[0]));

			Assert.AreEqual (1, _received.Count);
		}

		[Test]
		public void should_not_publish_cancelled_reminders()
		{
			var reminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "content", SystemTime.Now(), new byte[0]));

			Assert.AreEqual (0, _received.Count);
		}

		[Test]
		public void should_not_keep_track_of_cancellations_once_they_are_due()
		{
			var reminderId = Guid.NewGuid ();

			_cancellationManager.Handle (new ReminderMessage.Cancel (reminderId));
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "content", SystemTime.Now(), new byte[0]));
			//will handle this message the second time because it has been removed from the CancellationManagers internal list
			_cancellationManager.Handle(new ReminderMessage.Due(reminderId, "deliveryUrl", "content", SystemTime.Now(), new byte[0]));

			Assert.AreEqual (1, _received.Count);
		}
	}

	public class FakeLogger : ILogger
	{
		private string _lastMessage;
		private readonly Action<Level, string> _logDelegate;

		public FakeLogger ()
		{
			//empty
		}

		public FakeLogger (Action<Level, string> logDelegate)
		{
			_logDelegate = logDelegate;
		}

		public string LastLoggedMessage {
			get { return _lastMessage; }
		}

		public void Configure (System.Collections.Generic.IDictionary<string, object> settings)
		{
			throw new NotImplementedException ();
		}

		public void Log (Level level, string message, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			_lastMessage = message;
			if(_logDelegate != null)
				_logDelegate (level, message);
		}

		public void Log (Level level, LogInfo logInfo, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			throw new NotImplementedException ();
		}

		public void LogException (Level level, Exception ex, string message, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			throw new NotImplementedException ();
		}

		public void LogException (Level level, Exception ex, LogInfo logInfo, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			throw new NotImplementedException ();
		}
	}
}

