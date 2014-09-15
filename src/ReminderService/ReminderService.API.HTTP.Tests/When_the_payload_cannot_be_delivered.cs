using NUnit.Framework;
using System;
using ReminderService.Messages;
using Nancy;
using Nancy.Testing;
using System.Text;
using ReminderService.Common;
using RestSharp;
using System.Linq;
using ReminderService.API.HTTP.Models;

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

			var successfulResponse = new RestResponse {
				StatusCode = System.Net.HttpStatusCode.Created,
				ResponseStatus = ResponseStatus.Completed,
			};

			SetHttpClientResponses (new [] {
				failedResponse,
				failedResponse,
				failedResponse,
				failedResponse,
				successfulResponse,
			});

			var scheduleRequest = new ScheduleReminder (
				UtcNow.Add(2.Hours()).ToString("o"),
				"http://delivery",
				"application/json",
				"utf8",
				"http",
				"{\"property1\": \"payload\"}".AsUtf8EncodedByteArray(),
				3,
				UtcNow.Add(3.Hours()).ToString("o")
			);

			POST ("/reminders/", scheduleRequest);

			should_return_a_reminder_id ();
		}

		private void should_return_a_reminder_id()
		{
			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			var responseBody = Response.Body.DeserializeJson<ScheduledResponse>();
			_reminderId = responseBody.ReminderId;
		}

		[Test]
		public void should_deliver_to_the_deadletterqueue()
		{
			Assert.AreEqual (0, AllInterceptedHttpRequests.Count);

			AdvanceTimeBy (2.Hours ());
			FireScheduler ();

			GET ("/reminders/", _reminderId);
			Assert.That (ResponseBody.Contains ("RedeliveryAttempts\":1"));

			AdvanceTimeBy (3.Hours ());
			FireScheduler ();

			GET ("/reminders/", _reminderId);
			Assert.That (ResponseBody.Contains ("RedeliveryAttempts\":4"));

			var deadLetterBody = LastInterceptedHttpRequest
				.Parameters
				.Where (p => p.Type == ParameterType.RequestBody)
				.Select (p => p.Value)
				.FirstOrDefault ();

			Assert.AreEqual ("http://deadletter/url", LastInterceptedHttpRequest.Resource);
		}
	}
}

