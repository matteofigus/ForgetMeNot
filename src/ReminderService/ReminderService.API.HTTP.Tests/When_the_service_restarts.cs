using NUnit.Framework;
using System;
using System.Collections.Generic;
using ReminderService.Test.Common;
using Nancy;
using Nancy.Testing;
using ReminderService.Messages;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_the_service_restarts : ServiceSpec<ReminderApiModule>
	{
		private List<Guid> _canceledReminderIds = new List<Guid> ();
		private List<Guid> _reminderIds = new List<Guid> ();

		[TestFixtureSetUp]
		public void Given_some_reminders_have_been_scheduled_and_cancelled_and_sent()
		{
			FreezeTime ();

			var reminders = Given_some_reminders_have_been_scheduled ();
			Given_some_reminders_have_been_cancelled (reminders);
			// send some reminders

			When_the_service_restarts ();
		}

		[Test]
		public void Then_should_contain_cancellations()
		{

		}

		[Test]
		public void Then_should_contain_current_reminders()
		{

		}

		private void Given_some_reminders_have_been_scheduled()
		{
			// schedule some reminders
			var remindersToSchedule = MessageBuilders.BuildRemindersWithoutIds (10);
			foreach (var reminder in remindersToSchedule) {
				POST ("/reminders", reminder);
				Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
				_reminderIds.Add (Response.Body.DeserializeJson<ReminderMessage.ScheduledResponse> ().ReminderId);
			}

			return remindersToSchedule;
		}

		public void Given_some_reminders_have_been_cancelled(IEnumerable<ReminderMessage.Schedule> scheduledReminders)
		{
			// cancel some of those reminders
			var cancellations = (IEnumerable<ReminderMessage.Cancel>)MessageBuilders.BuildCancellationsAsSubsetOfReminders (3, scheduledReminders);
			foreach (var cancellation in cancellations) {
				DELETE ("/reminders", cancellation.ReminderId);
				Assert.AreEqual (HttpStatusCode.NoContent, Response.StatusCode);
			}
		}

		private void When_the_service_restarts()
		{
			RestartService();
		}
	}
}

