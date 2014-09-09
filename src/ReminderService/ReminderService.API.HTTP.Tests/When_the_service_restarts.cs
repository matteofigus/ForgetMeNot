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
using ReminderService.API.HTTP.Models;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_the_service_restarts : ServiceSpec<ReminderApiModule>
	{
		private List<Tuple<ScheduleReminder, Guid>> _scheduledReminders = new List<Tuple<ScheduleReminder, Guid>> ();
		private List<Guid> _canceledReminderIds = new List<Guid> ();
		private List<Guid> _sentCorrelatioIds = new List<Guid>();
		private List<Guid> _correlationIds;

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
				.NotIntersectWith (_sentCorrelatioIds, "The test rig received ReminderId's for cancelled Reminders");
		}

		[Test]
		public void Then_due_reminders_should_be_sent()
		{
			_sentCorrelatioIds.Should ().HaveCount (7);

			_sentCorrelatioIds
				.Should()
				.IntersectWith(
					_scheduledReminders
					.Select(r => 
						r.Item1.GetFakePayload().CorrelationId), "No reminders were received.");
		}

		private void Given_some_reminders_have_been_scheduled()
		{
			_correlationIds = Enumerable
				.Range (0, 10)
				.Select (i => Guid.NewGuid())
				.ToList();

			foreach (var id in _correlationIds) {
				var reminder = Helpers.BuildScheduleRequest (id);
				POST ("/reminders", reminder);
				Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);

				var reminderId = Response.Body.DeserializeJson<ScheduledResponse> ().ReminderId;
				_scheduledReminders.Add(new Tuple<ScheduleReminder, Guid>(reminder, reminderId));
			}
		}

		private void Given_some_reminders_have_been_cancelled()
		{
			var cancellations = _scheduledReminders.Select (r => r.Item2).Take (3);
			foreach (var cancellation in cancellations) {
				DELETE ("/reminders/", cancellation);
				Assert.AreEqual (HttpStatusCode.NoContent, Response.StatusCode, 
					string.Format(
						"DELETE request for reminder [{0}] failed with HttpStatus [{1}] and '{2}'", 
						cancellation, Response.StatusCode, Response.Body.DeserializeJson<string>()));
				_canceledReminderIds.Add (cancellation);
			}
		}

		private void Get_reminderIds_that_were_sent()
		{
			_sentCorrelatioIds.AddRange(
				AllInterceptedHttpRequests.Select (request => 
					request.GetFakePayload().CorrelationId));
		}

		private void When_service_restarts()
		{
			RestartService();
		}

		private IEnumerable<string> GetReminderStates(IEnumerable<Guid> reminders)
		{
			foreach (var id in reminders) {
				//var response = _service.Get ("/reminders/" + id.ToString());
				GET ("/reminders", id);
				yield return ResponseBody;
			}
		}
	}
}

