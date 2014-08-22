﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Core.DeliverReminder;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Test.Common;
using RestSharp;

namespace ReminderService.Core.Tests.PublishReminders
{
	[TestFixture ()]
	public class When_routing_reminders__that_are_due
	{
		[Test ()]
		public void should_route_the_reminder_to_the_correct_handler ()
		{
			var published = new List<IMessage>();
			var fakeBus = new FakeBus (msg => published.Add(msg));
			var dueReminder = new ReminderMessage.Schedule (Guid.NewGuid (), DateTime.Now, "http://delivery/url", "", new byte[0]).AsDue();
			var router = new DeliveryRouter (fakeBus);
			router.AddHandler (DeliveryTransport.HTTP, new FakeDelivery((due) => {
				Assert.AreSame(dueReminder, due);
			}));
			router.AddHandler (DeliveryTransport.None, null);

			router.Handle (dueReminder);

			Assert.IsTrue(published.ContainsOne<ReminderMessage.Delivered>());
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void should_throw_if_no_handler_is_available()
		{
			var fakeBus = new FakeBus ();
			var fakeRestClient = new FakeRestClient (new []{ new RestResponse () });
			var router = new DeliveryRouter (fakeBus);
			router.AddHandler (DeliveryTransport.HTTP, new HTTPDelivery (fakeRestClient, "deadletterurl"));
			router.AddHandler (DeliveryTransport.None, null);

			router.Handle (new ReminderMessage.Schedule(Guid.NewGuid(), DateTime.Now, "rabbit://queue/name", "",new byte[0]).AsDue());
		}
	}
}

