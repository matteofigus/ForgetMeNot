using System;
using NUnit.Framework;
using Nancy.Testing;
using ReminderService.API.HTTP.Models;
using ReminderService.API.HTTP.Modules;
using ReminderService.Common;
using ReminderService.Messages;

namespace ReminderService.API.HTTP.Tests
{
	public class When_replicating_a_reminder : ServiceSpec<ReminderApiModule>
	{
		private Guid _reminderId;

		[TestFixtureSetUp]
		public void FixtureSetUp()
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

			POST ("/reminders", scheduleRequest, "replicated", "true");
		}

		[SetUp]
		public void should_return_a_reminder_id()
		{
			Assert.AreEqual (Nancy.HttpStatusCode.Created, Response.StatusCode);

			var responseBody = Response.Body.DeserializeJson<ScheduledResponse>();
			_reminderId = responseBody.ReminderId;

			Assert.AreNotEqual (Guid.Empty, _reminderId);
		}

		[Test]
		public void should_be_able_to_GET_the_replicated_reminder()
		{
			var response = _service.Get ("/reminders/" + _reminderId);
			var retrieved = response.Body.AsString ();

			Assert.IsNotNullOrEmpty (retrieved);
			Assert.IsFalse (retrieved.Contains("ErrorMessage"));
			Assert.IsTrue (retrieved.Contains(_reminderId.ToString()));
			Assert.IsTrue (retrieved.Contains ("Scheduled"));
		}
	}
}

