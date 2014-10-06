using NUnit.Framework;
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
		public void should_route_an_http_reminder_to_the_correct_handler ()
		{
			var published = new List<IMessage>();
			var fakeBus = new FakeBus (msg => published.Add(msg));
			var dueReminder = new ReminderMessage.Schedule (Guid.NewGuid (), DateTime.Now, "http://delivery/url", "", ReminderMessage.ContentEncodingEnum.utf8, ReminderMessage.TransportEnum.http, new byte[0], 0).AsDue();
			var router = new DeliveryRouter (fakeBus, "deadletterurl");
			bool isDeliveredHttp = false;
			router.AddHandler (DeliveryTransport.HTTP, new FakeDelivery((due) => {
				Assert.AreSame(dueReminder.Reminder, due);
				isDeliveredHttp = true;
			}));
			bool isDeliveredRabbit = false;
			router.AddHandler (DeliveryTransport.RabbitMq, new FakeDelivery((due) => {
				Assert.AreSame(dueReminder.Reminder, due);
				isDeliveredRabbit = true;
			}));

			router.AddHandler (DeliveryTransport.None, null);
			router.Handle (dueReminder);

			Assert.IsTrue(published.DoesNotContainAnyThing());

			Assert.IsTrue(isDeliveredHttp);
			Assert.IsFalse(isDeliveredRabbit);
		}

		[Test()]
		public void should_route_a_rabbit_reminder_to_the_correct_handler ()
		{
			var published = new List<IMessage>();
			var fakeBus = new FakeBus(msg => published.Add(msg));
			var dueReminder = new ReminderMessage.Schedule(Guid.NewGuid (), DateTime.Now, "amqp://delivery/url", "", ReminderMessage.ContentEncodingEnum.utf8, ReminderMessage.TransportEnum.rabbitmq, new byte[0], 0).AsDue();
			var router = new DeliveryRouter(fakeBus, "deadletterurl");
			bool isDeliveredHttp = false;
			router.AddHandler(DeliveryTransport.HTTP, new FakeDelivery((due) => {
				Assert.AreSame(dueReminder.Reminder, due);
				isDeliveredHttp = true;
			}));
			bool isDeliveredRabbit = false;
			router.AddHandler(DeliveryTransport.RabbitMq, new FakeDelivery((due) => {
				Assert.AreSame(dueReminder.Reminder, due);
				isDeliveredRabbit = true;
			}));

			router.AddHandler(DeliveryTransport.None, null);
			router.Handle(dueReminder);

			Assert.IsTrue(published.DoesNotContainAnyThing());

			Assert.IsFalse(isDeliveredHttp);
			Assert.IsTrue(isDeliveredRabbit);
		}

		[Test]
		[ExpectedException(typeof(NotSupportedException))]
		public void should_throw_if_no_handler_is_available()
		{
			var fakeBus = new FakeBus ();
			var fakeRestClient = new FakeRestClient (new []{ new RestResponse () });
			var router = new DeliveryRouter (fakeBus, "deadletterurl");
			router.AddHandler (DeliveryTransport.HTTP, new HTTPDelivery (fakeRestClient));
			router.AddHandler (DeliveryTransport.None, null);

			router.Handle (new ReminderMessage.Schedule(Guid.NewGuid(), DateTime.Now, "amqp://queue/name", "",ReminderMessage.ContentEncodingEnum.utf8,ReminderMessage.TransportEnum.rabbitmq, new byte[0], 0).AsDue());
		}
	}
}

