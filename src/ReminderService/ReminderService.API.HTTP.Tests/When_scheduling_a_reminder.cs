using NUnit.Framework;
using System;
using ReminderService.Messages;
using ReminderService.Common;
using ReminderService.Test.Common;
using RestSharp;
using Nancy;
using Nancy.Testing;
using System.Text;
using ReminderService.API.HTTP.Models;

namespace ReminderService.API.HTTP.Tests
{
	public class When_scheduling_a_reminder : ServiceSpec<ReminderApiModule>
	{
		[TestFixtureSetUp]
		public void when_scheduling_a_reminder()
		{
			FreezeTime ();

			var scheduleRequest = new ScheduleReminder (
				Now.Add(2.Hours()).ToString("o", System.Globalization.CultureInfo.InvariantCulture),
				"http://delivery",
				"application/json",
				"utf8",
				"http",
				"{\"property1\": \"payload\"}".AsUtf8EncodedByteArray(),
				0,
				string.Empty
			);

			POST ("/reminders", scheduleRequest);
		}

		[Test]
		public void should_return_a_reminder_id()
		{
			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			var responseBody = Response.Body.DeserializeJson<ScheduledResponse>();
			Assert.AreNotEqual (Guid.Empty, responseBody.ReminderId);
		}

		[Test]
		public void should_deliver_the_reminder_when_due()
		{
			AdvanceTimeBy (2.Hours());
			FireScheduler ();
			Assert.IsNotNull (LastInterceptedHttpRequest);
			Assert.AreEqual ("http://delivery", LastInterceptedHttpRequest.Resource);
			//hmmm, how to get the payload from the request that was made by the HTTPDelivery component?
		}
	}
}

