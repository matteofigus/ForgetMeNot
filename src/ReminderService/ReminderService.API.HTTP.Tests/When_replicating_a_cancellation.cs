using System;
using NUnit.Framework;
using ReminderService.API.HTTP.Modules;
using ReminderService.API.HTTP.Models;
using ReminderService.Common;
using Nancy.Testing;
using Nancy;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_replicating_a_cancellation : ServiceSpec<ReminderApiModule>
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

			Assert.AreEqual (Nancy.HttpStatusCode.Created, Response.StatusCode);

			_reminderId = Response.Body.DeserializeJson<ScheduledResponse>().ReminderId;
		}

		[SetUp]
		public void When_the_reminder_is_cancelled()
		{
			DELETE ("/reminders/", _reminderId, "replicated", "true");

			Assert.AreEqual (HttpStatusCode.NoContent, Response.StatusCode);
		}

		[Test]
		public void reminder_should_be_in_a_cancelled_state()
		{
			var response = _service.Get ("/reminders/" + _reminderId);
			var retrieved = response.Body.AsString ();

			Assert.IsNotNullOrEmpty (retrieved);
			Assert.IsFalse (retrieved.Contains("ErrorMessage"));
			Assert.IsTrue (retrieved.Contains(_reminderId.ToString()));
			Assert.IsTrue (retrieved.Contains ("Canceled"));
		}
	}
}

