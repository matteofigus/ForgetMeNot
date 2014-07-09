using NUnit.Framework;
using System;
using Nancy;
using Nancy.Testing;
using ReminderService.Messages;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public class WhenSchedulingAReminder
	{
		[Test ()]
		public void Should_return_a_reminderId ()
		{
			// Given
			var bootstrapper = new Bootstrapper();
			var browser = new Browser(bootstrapper);

			// When
			var requestBody = new ReminderMessages.ScheduleReminder (
				                  "deliveryurl",
				                  "application/json",
				                  DateTime.Now.AddDays (1),
				                  new byte[0]
			                  );
			var result = browser.Post("/reminders", with => {
				with.JsonBody(requestBody);
			});

			// Then
			Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
		}

		[Test]
		public void Should_fail_when_TimoutAt_value_is_in_the_past()
		{
			// Given
			var bootstrapper = new Bootstrapper();
			var browser = new Browser(bootstrapper);

			// When
			var requestBody = new ReminderMessages.ScheduleReminder (
				"deliveryurl",
				"application/json",
				DateTime.Now.AddMinutes(-1),
				new byte[0]
			);
			var result = browser.Post("/reminders", with => {
				with.JsonBody(requestBody);
			});

			// Then
			Assert.AreEqual(HttpStatusCode.Forbidden, result.StatusCode);
			Assert.AreEqual ("TimeoutAt was in the past", result.Body);
		}
	}
}

