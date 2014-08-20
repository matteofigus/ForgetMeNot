using NUnit.Framework;
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
		private ReminderMessage.Schedule _getResponse;

		[TestFixtureSetUp]
		public void Given_a_reminder_exists_in_the_service()
		{
			var scheduleRequest = new ReminderMessage.Schedule (
				"http://delivery",
				"http://deadletter",
				"application/json",
				Now.Add(2.Hours()),
				Encoding.UTF8.GetBytes ("{\"property1\": \"payload\"}")
			);

			POST ("/reminders", scheduleRequest);

			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			_reminderId = Response.Body.DeserializeJson<ReminderMessage.ScheduledResponse>().ReminderId;
			Assert.AreNotEqual (Guid.Empty, _reminderId);
		}

		[SetUp]
		public void When_a_GET_request_is_made ()
		{
			GET ("/reminders", _reminderId);
		}

		[Test]
		public void Then_the_response_is_a_200()
		{
			Assert.AreEqual (HttpStatusCode.OK, Response.StatusCode);
		}

		[Test]
		public void Then_the_response_contains_the_expected_reminder()
		{
			_getResponse = Response.Body.DeserializeJson<ReminderMessage.Schedule> ();
			Assert.IsNotNull (_getResponse);
			Assert.AreEqual (_reminderId, _getResponse.ReminderId);
		}
	}
}

