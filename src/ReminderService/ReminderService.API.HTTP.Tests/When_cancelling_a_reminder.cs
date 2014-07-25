using NUnit.Framework;
using System;
using ReminderService.Messages;
using Nancy;
using ReminderService.Common;
using System.Text;

namespace ReminderService.API.HTTP.Tests
{
	public class When_cancelling_a_reminder : ServiceSpec<ReminderApiModule>
	{
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
		}

		[SetUp]
		public void When_the_reminder_is_cancelled()
		{
			var cancelationRequest = new ReminderMessage.Cancel (Guid.NewGuid());

			POST ("/reminders/cancellations", cancelationRequest);

			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
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

