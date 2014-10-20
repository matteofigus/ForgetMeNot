using NUnit.Framework;
using System;
using ReminderService.Messages;
using Nancy;
using Nancy.Testing;
using ReminderService.Common;
using System.Text;
using ReminderService.API.HTTP.Models;
using ReminderService.API.HTTP.Modules;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_getting_a_reminder : ServiceSpec<ReminderApiModule>
	{
		private Guid _reminderId;

		[TestFixtureSetUp]
		public void Given_a_reminder_exists_in_the_service()
		{
			var scheduleRequest = new ScheduleReminder (
				Now.Add(2.Hours()).ToString("o"),
				"http://delivery",
				"application/json",
				"utf8",
				"http",
				"{\"property1\": \"payload\"}".AsUtf8EncodedByteArray(),
				0,
				string.Empty
			);

			POST ("/reminders", scheduleRequest);

			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			_reminderId = Response.Body.DeserializeJson<ScheduledResponse>().ReminderId;
			Assert.AreNotEqual (Guid.Empty, _reminderId);
		}

		[SetUp]
		public void When_a_GET_request_is_made ()
		{
			GET ("/reminders/", _reminderId);
		}

		[Test]
		public void Then_the_response_is_a_200()
		{
			Assert.AreEqual (HttpStatusCode.OK, Response.StatusCode);
		}

		[Test]
		public void Then_the_response_contains_the_expected_reminder()
		{
			//TOOD: Encoding the payload should be totally transparent to the client; it is an internal concern of the service
			//the client should send payload as json string, and should get payload back as json string -> 
			//client should not know about UTF8 encoding
			//need to change this test when I fix the encoding
			var body = Response.Body.AsString ();
			//var reminderId = Response.Body.DeserializeJson<RequestResponse.CurrentReminderState> ().Reminder.ReminderId;
			//_getResponse = Response.Body.DeserializeJson<RequestResponse.CurrentReminderState> ();
			Assert.IsNotNullOrEmpty (body);
			Assert.IsTrue (body.Contains(_reminderId.ToString()));
		}
	}
}

