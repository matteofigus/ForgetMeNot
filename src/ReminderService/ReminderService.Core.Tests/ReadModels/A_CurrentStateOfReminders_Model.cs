using NUnit.Framework;
using System;
using ReminderService.Core.ReadModels;
using ReminderService.Messages;
using ReminderService.Test.Common;
using System.Collections.Generic;
using ReminderService.Common;

namespace ReminderService.Core.Tests.ReadModels
{
	[TestFixture]
	public class A_CurrentStateOfReminders_Model
	{
		private CurrentStateOfReminders _reminderStates;
		private List<ReminderMessage.Schedule> _reminders;

		[SetUp]
		public void Given_a_bunch_of_remimders()
		{
			_reminderStates = new CurrentStateOfReminders ();
			_reminders = new List<ReminderMessage.Schedule> (MessageBuilders.BuildReminders (5));
			foreach (var reminder in _reminders) {
				_reminderStates.Handle (
					new Envelopes.Journaled<ReminderMessage.Schedule>(reminder)
				);
			}
		}

		[Test]
		public void Should_keep_track_of_reminders ()
		{
			var reminderId = _reminders[2].ReminderId;
			var query = new RequestResponse.GetReminderState (reminderId);
			var response = _reminderStates.Handle (query);

			Assert.IsTrue (response.HasValue);
			Assert.AreEqual (reminderId, response.Value.Reminder.ReminderId);
			Assert.AreEqual (RequestResponse.ReminderStatusEnum.Scheduled, response.Value.Status);
		}

		[Test]
		public void Should_keep_track_of_cancellations ()
		{
			var reminderId = _reminders[1].ReminderId;
			_reminderStates.Handle (new Envelopes.Journaled<ReminderMessage.Cancel> (
				new ReminderMessage.Cancel (reminderId))
			);

			var query = new RequestResponse.GetReminderState (reminderId);
			var response = _reminderStates.Handle (query);

			Assert.IsTrue (response.HasValue);
			Assert.AreEqual (reminderId, response.Value.Reminder.ReminderId);
			Assert.AreEqual (RequestResponse.ReminderStatusEnum.Canceled, response.Value.Status);
		}

		[Test]
		public void Should_keep_track_of_delivered_reminders ()
		{
			var reminderId = _reminders[4].ReminderId;
			_reminderStates.Handle (new ReminderMessage.Delivered(reminderId, SystemTime.UtcNow()));

			var query = new RequestResponse.GetReminderState (reminderId);
			var response = _reminderStates.Handle (query);

			Assert.IsTrue (response.HasValue);
			Assert.AreEqual (reminderId, response.Value.Reminder.ReminderId);
			Assert.AreEqual (RequestResponse.ReminderStatusEnum.Delivered, response.Value.Status);
		}

		[Test]
		public void Should_keep_track_of_undeliverable_reminders ()
		{
			var reminderId = _reminders[4].ReminderId;
			_reminderStates.Handle (new ReminderMessage.Undeliverable(_reminders[4], "404 - Not Found"));

			var query = new RequestResponse.GetReminderState (reminderId);
			var response = _reminderStates.Handle (query);

			Assert.IsTrue (response.HasValue);
			Assert.AreEqual (reminderId, response.Value.Reminder.ReminderId);
			Assert.AreEqual (RequestResponse.ReminderStatusEnum.Undeliverable, response.Value.Status);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Should_return_an_empty_Maybe_if_the_reminder_does_not_exist()
		{
			var reminderId = Guid.NewGuid();
			var query = new RequestResponse.GetReminderState (reminderId);
			var response = _reminderStates.Handle (query);

			Assert.IsFalse (response.HasValue);
			Assert.IsNull(response.Value);
		}
	}
}

