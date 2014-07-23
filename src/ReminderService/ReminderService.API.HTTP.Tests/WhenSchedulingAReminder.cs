using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Nancy;
using Nancy.Testing;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.API.HTTP.BootStrap;
using ReminderService.Core.DeliverReminder;
using ReminderService.Test.Common;
using RestSharp;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public class WhenSchedulingAReminder
	{
		[Test]
		public void Should_return_a_400_when_the_request_is_not_valid()
		{
			// Given
			var bootstrapper = new DefaultNancyBootstrapper();
			var browser = new Browser(bootstrapper);

			// When
			var requestBody = new ReminderMessage.Schedule (
				"",
				"deadletterurl",
				"application/json",
				DateTime.Now.AddDays (1),
				new byte[1] {01}
			);
			var result = browser.Post("/reminders", with => {
				with.JsonBody(requestBody);
			});

			// Then
			Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
			var responseBody = result.Body.DeserializeJson<IEnumerable<dynamic>> ().ToList();
			Assert.AreEqual ("'Delivery Url' should not be empty.", responseBody[0]["ErrorMessage"]);
		}

		[Test ()]
		public void Should_return_a_reminderId ()
		{
			// Given
			var browser = new Browser (new BootStrapper());

			// When
			var requestBody = new ReminderMessage.Schedule (
				                  "deliveryurl",
				                  "deadletterurl",
				                  "application/json",
				                  DateTime.Now.AddDays (1),
				                  Encoding.UTF8.GetBytes ("payload")
			                  );
			var result = browser.Post("/reminders", with => {
				with.JsonBody(requestBody);
			});

			// Then
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
			var responseBody = result.Body.DeserializeJson<ReminderMessage.ScheduledResponse>();
			Assert.AreNotEqual (Guid.Empty, responseBody.ReminderId);
		}

		[Test]
		public void should_deliver_the_payload_when_due()
		{
			var now = SystemTime.FreezeTime ();
			var fakeRestClient = new FakeRestClient (new []{ new RestResponse{ StatusCode = System.Net.HttpStatusCode.Created } });
			var browser = new Browser (new BootStrapper());

			// When
			var requestBody = new ReminderMessage.Schedule (
				"deliveryurl",
				"deadletterurl",
				"application/json",
				now.AddMinutes (1),
				Encoding.UTF8.GetBytes ("payload")
			);
			var result = browser.Post("/reminders", with => {
				with.JsonBody(requestBody);
			});

			// Then
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
			var responseBody = result.Body.DeserializeJson<ReminderMessage.ScheduledResponse>();
			Assert.AreNotEqual (Guid.Empty, responseBody.ReminderId);

			//advance time to trigger the reminder
			SystemTime.Set (now.AddMinutes (1));
			Assert.IsNotNull (fakeRestClient.LastRequest);
		}
	}
}

