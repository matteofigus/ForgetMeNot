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
using System.Threading;
using ReminderService.Router;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public class When_a_schedule_request_is_not_valid
	{
		[Test]
		public void Should_return_a_400()
		{
			// Given
			var bootstrapper = new DefaultNancyBootstrapper();
			var browser = new Browser(bootstrapper);

			// When
			var requestBody = new ReminderMessage.Schedule (
				"",
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
	}
}

