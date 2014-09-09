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
using System.Net;

namespace ReminderService.Core.Tests.PublishReminders
{
	[TestFixture]
	public class An_HttpPublisher
	{
		private bool _sendSucceeded;
		private bool _sendFailed;

		[SetUp]
		public void BeforeEach()
		{
			_sendSucceeded = false;
			_sendFailed = false;
		}

		[Test]
		public void should_send_reminders_to_the_DeliveryUrl ()
		{
			//arrange
			var payload = new TestPayload()
				{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = new ReminderMessage.Schedule (
				Guid.NewGuid (), SystemTime.Now (), "http://delivery/url","content","utf8",ReminderMessage.TransportEnum.http,  payload.AsUtf8Encoding(), 0);
			var expectedResponse = new RestResponse { ResponseStatus = ResponseStatus.Completed, StatusCode = HttpStatusCode.Created };
			var fakeClient = new FakeRestClient (new [] {expectedResponse});
			var publisher = new HTTPDelivery (fakeClient);

			//act
			publisher.Send (due, due.DeliveryUrl, OnSuccessfulSend, OnFailedSend);

			//assert
			Assert.IsTrue (_sendSucceeded);
			Assert.IsNotNull (fakeClient.LastRequest);
			Assert.AreEqual ("http://delivery/url", fakeClient.LastRequest.Resource);
		}

		[Test]
		public void should_invoke_the_failed_callback_if_the_DeliveryUrl_fails()
		{
			//arrange
			var payload = new TestPayload()
			{ Property1 = "string property", Property2 = 42, Property3 = SystemTime.Now () };
			var due = new ReminderMessage.Schedule (
				Guid.NewGuid (), SystemTime.Now (), "http://delivery/url","application/json","utf8",ReminderMessage.TransportEnum.http,  payload.AsUtf8Encoding(), 0);
			var response = new RestResponse { ResponseStatus = ResponseStatus.Error };
			var fakeClient = new FakeRestClient (new [] {response});
			var publisher = new HTTPDelivery (fakeClient);

			//act
			publisher.Send (due, due.DeliveryUrl, OnSuccessfulSend, OnFailedSend);

			//assert
			Assert.IsTrue (_sendFailed);
		}

		private void OnSuccessfulSend(ReminderMessage.Schedule sent)
		{
			_sendSucceeded = true;
		}

		private void OnFailedSend(ReminderMessage.Schedule failed, string error)
		{
			_sendFailed = true;
		}
	}
}

