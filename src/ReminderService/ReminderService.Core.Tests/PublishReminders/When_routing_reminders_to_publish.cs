using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.DeliverReminder;
using ReminderService.Core.Tests.Helpers;

namespace ReminderService.Core.Tests
{
	[TestFixture ()]
	public class When_routing_reminders_to_publish
	{
		[Test ()]
		public void should_route_the_reminder_to_the_correct_handler ()
		{
			var logger = new FakeLogger ();
			var called = false;
			Func<ReminderMessage.DueReminderNotCanceled, bool> httpHandler = (due) => {
				called = true;
				return true;
			};
			Func<ReminderMessage.DueReminderNotCanceled, bool> anotherHandler = (due) => {
				Assert.Fail();
				return true;
			};
			var handlers = new []{ httpHandler, anotherHandler };
			var router = new DeliveryRouter (logger, handlers);

			router.Handle (new ReminderMessage.DueReminderNotCanceled(Guid.NewGuid(), "", "", "", DateTime.Now, new byte[0]));

			Assert.IsTrue (called);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void should_throw_if_no_handler_is_available()
		{
			var logger = new FakeLogger ();
			Func<ReminderMessage.DueReminderNotCanceled, bool> anotherHandler = (due) => {
				return false;
			};
			var handlers = new []{ anotherHandler };
			var router = new DeliveryRouter (logger, handlers);

			router.Handle (new ReminderMessage.DueReminderNotCanceled(Guid.NewGuid(), "", "", "", DateTime.Now, new byte[0]));
		}
	}

}

