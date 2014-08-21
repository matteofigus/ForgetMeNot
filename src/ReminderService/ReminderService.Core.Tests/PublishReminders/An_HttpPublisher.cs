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
			var due = 
				new ReminderMessage.Due (
					Guid.NewGuid (), "http://delivery/url","content", SystemTime.Now (), payload.AsUtf8Encoding());
			var expectedResponse = new RestResponse { ResponseStatus = ResponseStatus.Completed };
			var fakeClient = new FakeRestClient (new [] {expectedResponse});
			var publisher = new HTTPDelivery (fakeClient, "deadletterurl");

			//act
			publisher.Send (due);

			//assert
			Assert.IsNotNull (fakeClient.LastRequest);
			Assert.AreEqual ("http://delivery/url", fakeClient.LastRequest.Resource);
		}

		[Test]
		public void should_send_reminders_to_the_DeadLetterUrl_if_the_DeliveryUrl_fails()
		{
			//arrange
			const string deadLetterUrl = "http://deadletter/url";
			var payload = new TestPayload()
			{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = 
				new ReminderMessage.Due (
					Guid.NewGuid (), "http://delivery/url","application/json", SystemTime.Now (), payload.AsUtf8Encoding());
			var firstResponse = new RestResponse { ResponseStatus = ResponseStatus.Error };
			var secondResponse = new RestResponse { ResponseStatus = ResponseStatus.Completed };
			var fakeClient = new FakeRestClient (new [] {firstResponse, secondResponse});
			var publisher = new HTTPDelivery (fakeClient, deadLetterUrl);

			//act
			publisher.Send (due);

			//assert
			Assert.AreEqual (deadLetterUrl, fakeClient.LastRequest.Resource);
		}

		[Test]
		[ExpectedException(typeof(ReminderUndeliverableException<ReminderMessage.Due>))]
		public void should_throw_if_it_can_not_deliver_the_reminder_at_all()
		{
			//arrange
			var payload = new TestPayload()
			{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = 
				new ReminderMessage.Due (
					Guid.NewGuid (), "http://delivery/url","application/json", SystemTime.Now (), payload.AsUtf8Encoding());
			var expectedResponse = new RestResponse { ResponseStatus = ResponseStatus.Error };
			var fakeClient = new FakeRestClient (new [] {expectedResponse});
			var publisher = new HTTPDelivery (fakeClient, "http://deadletter/url");

			//act
			publisher.Send (due);
		}
	}
}

