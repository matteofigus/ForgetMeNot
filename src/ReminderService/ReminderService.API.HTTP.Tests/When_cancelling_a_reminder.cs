using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Test.Common;
using RestSharp;

namespace ReminderService.API.HTTP.Tests
{
	public class When_cancelling_a_reminder : ServiceSpec<ReminderApiModule>
	{
		private Guid _canceledReminderId;
		private List<Guid> _reminderIds = new List<Guid> ();

		[TestFixtureSetUp]
		public void Given_some_reminders_have_been_scheduled()
		{
			FreezeTime ();

			var remindersToSchedule = MessageBuilders.BuildRemindersWithoutIds (10);

			foreach (var reminder in remindersToSchedule) {
				POST ("/reminders", reminder);
				Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
				_reminderIds.Add (Response.Body.DeserializeJson<ScheduledResponse> ().ReminderId);
			}
		}

		[SetUp]
		public void When_the_reminder_is_cancelled()
		{
			_canceledReminderId = _reminderIds.First ();
			DELETE ("/reminders", _canceledReminderId);

			Assert.AreEqual (HttpStatusCode.NoContent, Response.StatusCode);
		}

		[Test]
		public void Then_should_not_receive_the_reminder_when_it_is_due()
		{
			AdvanceTimeBy (2.Hours());
			FireScheduler ();
			Assert.AreEqual (9, AllDeliveredHttpRequests.Count);
			Assert.IsTrue(
				AllDeliveredHttpRequests.DoesNotContain<IRestRequest> (r => r.Resource.EndsWith("0"))); //since we are cancelling the first reminder in the list, then the message with delivery url 'http://deliveryurl/0' should not exist in this list. Cant think of a better way to do this right now!
		}
	}
}

