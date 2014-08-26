using NUnit.Framework;
using System;
using ReminderService.Messages;
using Nancy;
using Nancy.Testing;
using System.Text;
using ReminderService.Common;
using RestSharp;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_the_payload_cannot_be_delivered : ServiceSpec<ReminderApiModule>
	{
		private Guid _reminderId;

		[TestFixtureSetUp]
		public void when_scheduling_a_reminder()
		{
			FreezeTime ();

			//the response to be returned from the delivery endpoint -> makes our payload undeliverable
			var failedResponse = new RestResponse {
				StatusCode = System.Net.HttpStatusCode.NotFound,
				ResponseStatus = ResponseStatus.Completed,
			};
			SetHttpClientResponse (failedResponse);

			var scheduleRequest = new ReminderMessage.Schedule (
				Now.Add(2.Hours()),
				"http://delivery",
				"application/json",
				Encoding.UTF8.GetBytes ("{\"property1\": \"payload\"}"),
				3,
				Now.Add(60.Minutes())
			);

			POST ("/reminders", scheduleRequest);
		}

		[Test]
		public void should_return_a_reminder_id()
		{
			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			var responseBody = Response.Body.DeserializeJson<ScheduledResponse>();
			Assert.AreNotEqual (Guid.Empty, responseBody.ReminderId);
			_reminderId = responseBody.ReminderId;
		}

		[Test]
		public void should_deliver_to_the_deadletterqueue()
		{
			Assert.AreEqual (0, AllDeliveredHttpRequests.Count);

			AdvanceTimeBy (10.Minutes ());
			FireScheduler ();

			GET("/reminders/", _reminderId);

		}
	}
}

