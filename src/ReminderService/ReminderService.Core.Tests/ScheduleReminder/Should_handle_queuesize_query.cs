using System;
using NUnit.Framework;
using ReminderService.Messages;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Test.Common;
using ReminderService.Common;

namespace ReminderService.Core.Tests.ScheduleReminder
{
	[TestFixture]
	public class Should_handle_queuesize_query
	{
		private Scheduler _scheduler;

		[TestFixtureSetUp]
		public void given_reminders_are_scheduled()
		{
			_scheduler = new Scheduler (new FakeBus(), new TestTimer());

			_scheduler.Handle (new SystemMessage.Start ());

			_scheduler.Handle (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					SystemTime.UtcNow().AddDays(1),
					"http://deliveryUrl/1",
					"content/type",
					ReminderMessage.ContentEncodingEnum.utf8,
					ReminderMessage.TransportEnum.http,
					new byte[0], 0)));

			_scheduler.Handle (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					SystemTime.UtcNow().AddDays(1),
					"http://deliveryUrl/2",
					"content/type",
					ReminderMessage.ContentEncodingEnum.utf8,
					ReminderMessage.TransportEnum.http,
					new byte[0], 0)));

			_scheduler.Handle (new Envelopes.Journaled<ReminderMessage.Schedule> (
				new ReminderMessage.Schedule (
					SystemTime.UtcNow().AddDays(1),
					"http://deliveryUrl/3",
					"content/type",
					ReminderMessage.ContentEncodingEnum.utf8,
					ReminderMessage.TransportEnum.http,
					new byte[0], 0)));
		}

		[Test]
		public void Should_return_the_queuesize()
		{
			var response = _scheduler.Handle (new QueryResponse.GetQueueStats());

			Assert.AreEqual (3, response.QueueSize);
		}
	}
}

