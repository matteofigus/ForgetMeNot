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
	public class An_HttpPublisher : RoutableBase, IConsume<ReminderMessage.Sent>
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
			var payload = Encoding.UTF8.GetBytes ("payload as plain/text");
			var due = new ReminderMessage.Due (Guid.NewGuid (), "http://delivery/url", "deadletterurl","content", SystemTime.Now (), payload);
			var expectedResponse = new RestResponse { ResponseStatus = ResponseStatus.Completed };
			var fakeClient = new FakeRestClient (expectedResponse);
			var publisher = new HTTPPublisher (fakeClient);

			//act
			publisher.Send (due);

			//assert
			Assert.AreEqual (1, Received.Count);
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

