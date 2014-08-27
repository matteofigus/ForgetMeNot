using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using ReminderService.Messages;
using ReminderService.Test.Common;
using Newtonsoft.Json;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_the_service_restarts : ServiceSpec<ReminderApiModule>
	{
		private List<Tuple<ReminderMessage.Schedule, Guid>> _scheduledReminders = new List<Tuple<ReminderMessage.Schedule, Guid>> ();
		private List<Guid> _canceledReminderIds = new List<Guid> ();
		private List<Guid> _sentReminderIds = new List<Guid>();

		[TestFixtureSetUp]
		public void Initialize_the_service()
		{
			FreezeTime ();

			Given_some_reminders_have_been_scheduled ();
			Given_some_reminders_have_been_cancelled ();

			When_service_restarts ();

			AdvanceTimeBy (1.Hours());
			FireScheduler ();

			Get_reminderIds_that_were_sent ();
		}

		[Test]
		public void Then_cancellations_should_not_be_sent()
		{
			_canceledReminderIds.Should ().HaveCount (3);
			_canceledReminderIds
				.Should ()
				.NotIntersectWith (_sentReminderIds, "The test rig received ReminderId's for cancelled Reminders");
		}

		[Test]
		public void Then_current_reminders_should_be_sent()
		{
			//failing because records coming out of PostGres are in UTC
			_sentReminderIds.Should ().HaveCount (7);

			_sentReminderIds
				.Should()
				.IntersectWith(_scheduledReminders.Select(r => r.Item1.GetFakePayload().CorrelationId), "No reminders were received.");
		}

		private void Given_some_reminders_have_been_scheduled()
		{
			var reminders = MessageBuilders.BuildRemindersWithoutIds (10);
			foreach (var reminder in reminders) {
				POST ("/reminders", reminder);
				Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);

				var reminderId = Response.Body.DeserializeJson<ScheduledResponse> ().ReminderId;
				_scheduledReminders.Add(new Tuple<ReminderMessage.Schedule, Guid>(reminder, reminderId));
			}
		}

		private void Given_some_reminders_have_been_cancelled()
		{
			var cancellations = _scheduledReminders.Select (r => r.Item2).Take (3);
			foreach (var cancellation in cancellations) {
				DELETE ("/reminders", cancellation);
				Assert.AreEqual (HttpStatusCode.NoContent, Response.StatusCode, 
					string.Format(
						"DELETE request for reminder [{0}] failed with HttpStatus [{1}] and '{2}'", 
						cancellation, Response.StatusCode, Response.Body.DeserializeJson<string>()));
				_canceledReminderIds.Add (cancellation);
			}
		}

		private void Get_reminderIds_that_were_sent()
		{
			_sentReminderIds.AddRange(
				AllInterceptedHttpRequests.Select (request => 
					request.GetFakePayload().CorrelationId));
		}

		private void When_service_restarts()
		{
			RestartService();
		}
	}
}

