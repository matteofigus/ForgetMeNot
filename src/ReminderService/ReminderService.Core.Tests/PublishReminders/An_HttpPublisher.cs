using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Core.DeliverReminder;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Core.Tests.Helpers;
using RestSharp;
using ReminderService.Test.Common;
using TestPayload = ReminderService.Core.Tests.Helpers.TestPayload;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core.Tests.PublishReminders
{
	[TestFixture]
	public class An_HttpPublisher
	{
		[Test]
		public void should_send_reminders_to_the_DeliveryUrl ()
		{
			//arrange
			var payload = new TestPayload()
				{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = new ReminderMessage.Schedule (
					Guid.NewGuid (), SystemTime.Now (), "http://delivery/url","content", payload.AsUtf8Encoding(), 0);
			var expectedResponse = new RestResponse { ResponseStatus = ResponseStatus.Completed };
			var fakeClient = new FakeRestClient (new [] {expectedResponse});
			var publisher = new HTTPDelivery (fakeClient, new FakeBus());

			//act
			publisher.Send (due, due.DeliveryUrl);

			//assert
			Assert.IsNotNull (fakeClient.LastRequest);
			Assert.AreEqual ("http://delivery/url", fakeClient.LastRequest.Resource);
		}

		[Test]
		public void should_send_an_Undelivered_message_if_the_DeliveryUrl_fails()
		{
			//arrange
			var payload = new TestPayload()
			{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = new ReminderMessage.Schedule (
					Guid.NewGuid (), SystemTime.Now (), "http://delivery/url","application/json", payload.AsUtf8Encoding(), 0);
			var response = new RestResponse { ResponseStatus = ResponseStatus.Error };
			var fakeClient = new FakeRestClient (new [] {response});
			IMessage receivedMessage = null;
			var fakeBus = new FakeBus (msg => receivedMessage = msg);
			var publisher = new HTTPDelivery (fakeClient, fakeBus);

			//act
			publisher.Send (due, due.DeliveryUrl);

			//assert
			Assert.That(receivedMessage, Is.InstanceOf<ReminderMessage.Undelivered>());
		}
	}
}

