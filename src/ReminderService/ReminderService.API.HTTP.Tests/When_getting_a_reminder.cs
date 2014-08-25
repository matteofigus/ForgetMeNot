﻿using NUnit.Framework;
using System;
using ReminderService.Messages;
using Nancy;
using Nancy.Testing;
using ReminderService.Common;
using System.Text;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_getting_a_reminder : ServiceSpec<ReminderApiModule>
	{
		private Guid _reminderId;
		private RequestResponse.CurrentReminderState _getResponse;

		[TestFixtureSetUp]
		public void Given_a_reminder_exists_in_the_service()
		{
			var scheduleRequest = new ReminderMessage.Schedule (
				Now.Add(2.Hours()),
				"http://delivery",
				"application/json",
				Encoding.UTF8.GetBytes ("{\"property1\": \"payload\"}"),
				0
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

