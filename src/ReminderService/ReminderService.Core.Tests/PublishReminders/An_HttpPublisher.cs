using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Core.PublishReminders;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Core.Tests.Helpers;
using RestSharp;

namespace ReminderService.Core.Tests
{
	[TestFixture]
	public class An_HttpPublisher : RoutableTestBase, IConsume<ReminderMessage.Sent>
	{
		[SetUp]
		public void BeforeEach()
		{
			ClearReceived ();
		}

		[Test]
		public void should_send_reminders_to_the_DeliveryUrl ()
		{
			//arrange
			var payload = new TestPayload()
				{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = 
				new ReminderMessage.Due (
					Guid.NewGuid (), "http://delivery/url", "deadletterurl","content", SystemTime.Now (), payload.AsUtf8Encoding());
			var expectedResponse = new RestResponse { ResponseStatus = ResponseStatus.Completed };
			var fakeClient = new FakeRestClient (new [] {expectedResponse});
			var publisher = new HTTPPublisher (new FakeLogger(), fakeClient, Bus);

			//act
			publisher.Send (due);

			//assert
			Assert.AreEqual (1, Received.Count);
			Assert.IsInstanceOfType(typeof(ReminderMessage.Sent), Received.First());
			Assert.AreEqual ("http://delivery/url", fakeClient.LastRequest.Resource);
		}

		[Test]
		public void should_send_reminders_to_the_DeadLetterUrl_if_the_DeliveryUrl_fails()
		{
			//arrange
			var payload = new TestPayload()
			{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = 
				new ReminderMessage.Due (
					Guid.NewGuid (), "http://delivery/url", "http://deadletter/url","application/json", SystemTime.Now (), payload.AsUtf8Encoding());
			var firstResponse = new RestResponse { ResponseStatus = ResponseStatus.Error };
			var secondResponse = new RestResponse { ResponseStatus = ResponseStatus.Completed };
			var fakeClient = new FakeRestClient (new [] {firstResponse, secondResponse});
			var publisher = new HTTPPublisher (new FakeLogger(), fakeClient, Bus);

			//act
			publisher.Send (due);

			//assert
			Assert.AreEqual ("http://deadletter/url", fakeClient.LastRequest.Resource);
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
					Guid.NewGuid (), "http://delivery/url", "http://deadletter/url","application/json", SystemTime.Now (), payload.AsUtf8Encoding());
			var expectedResponse = new RestResponse { ResponseStatus = ResponseStatus.Error };
			var fakeClient = new FakeRestClient (new [] {expectedResponse});
			var publisher = new HTTPPublisher (new FakeLogger(), fakeClient, Bus);

			//act
			publisher.Send (due);
		}

		public An_HttpPublisher ()
		{
			Subscribe (this);
		}

		public void Handle (ReminderMessage.Sent msg)
		{
			Received.Add (msg);
		}
	}
}

