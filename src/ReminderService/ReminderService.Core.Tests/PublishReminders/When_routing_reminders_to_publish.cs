using NUnit.Framework;
using System;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.DeliverReminder;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Test.Common;
using RestSharp;

namespace ReminderService.Core.Tests
{
	[TestFixture ()]
	public class When_routing_reminders__that_are_due
	{
		[Test ()]
		public void should_route_the_reminder_to_the_correct_handler ()
		{
			var dueReminder = new ReminderMessage.Due (Guid.NewGuid (), "http://delivery/url", "", "", DateTime.Now, new byte[0]);
			var router = new DeliveryRouter ();
			router.AddHandler (DeliveryTransport.HTTP, new FakeDelivery((due) => {
				Assert.AreSame(dueReminder, due);
			}));
			router.AddHandler (DeliveryTransport.None, null);

			router.Handle (dueReminder);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void should_throw_if_no_handler_is_available()
		{
			var fakeRestClient = new FakeRestClient (new []{ new RestResponse () });
			var router = new DeliveryRouter ();
			router.AddHandler (DeliveryTransport.HTTP, new HTTPDelivery (fakeRestClient));
			router.AddHandler (DeliveryTransport.None, null);

			router.Handle (new ReminderMessage.Due(Guid.NewGuid(), "rabbit://queue/name", "", "", DateTime.Now, new byte[0]));
		}
	}
}

