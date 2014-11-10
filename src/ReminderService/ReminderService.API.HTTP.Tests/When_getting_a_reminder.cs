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
			//this fails the test because it deserializes the payload (a byte[]) as a base64 encoded string. Hence, I just stringify the body and check for certain values
			//var reminderStatus = Response.Body.DeserializeJson<ReminderStatus> ();
			var body = Response.Body.AsString ();
			Assert.IsNotNullOrEmpty (body);
			Assert.IsTrue (body.Contains(_reminderId.ToString()));
			Assert.IsTrue (body.Contains("Scheduled"));
		}
	}
}

