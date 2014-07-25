using NUnit.Framework;
using System;
using ReminderService.Messages;
using Nancy;
using ReminderService.Common;
using System.Text;
using Nancy.Testing;

namespace ReminderService.API.HTTP.Tests
{
	public class When_cancelling_a_reminder : ServiceSpec<ReminderApiModule>
	{
		private Guid _reminderId;

		[TestFixtureSetUp]
		public void Given_a_reminder_has_been_scheduled()
		{
			FreezeTime ();

			var scheduleRequest = new ReminderMessage.Schedule (
				"http://delivery",
				"http://deadletter",
				"application/json",
				Now.Add(2.Hours()),
				Encoding.UTF8.GetBytes ("payload")
			);

			POST ("/reminders", scheduleRequest);

			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);

			_reminderId = Response.Body.DeserializeJson<ReminderMessage.ScheduledResponse>().ReminderId;
		}

		[SetUp]
		public void When_the_reminder_is_cancelled()
		{
			DELETE ("/reminders", _reminderId);

			Assert.AreEqual (HttpStatusCode.NoContent, Response.StatusCode);
		}

		[Test]
		public void Then_should_not_receive_the_reminder_when_it_is_due()
		{
			AdvanceTimeBy (2.Hours());
			FireScheduler ();
			Assert.IsNull (DeliveryRequest);
		}
	}
}

