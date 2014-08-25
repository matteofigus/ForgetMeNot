using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;

namespace ReminderService.Core.DeliverReminder
{
	public class UndeliveredProcessManager : IConsume<ReminderMessage.Undelivered> //IConsume<Envelopes.Journaled<ReminderMessage.Undelivered>>
	{
		private readonly object _lockObject = new object ();
		private readonly IBus _bus;
		private readonly Dictionary<Guid, int> _retryAttempts = new Dictionary<Guid, int> ();

		public UndeliveredProcessManager (IBus bus)
		{
			Ensure.NotNull (bus, "bus");

			_bus = bus;
		}

		public void Handle (ReminderMessage.Undelivered undelivered)
		{
			if (undelivered.DoNotAttemptRedelivery ()) {
				_bus.Send (new ReminderMessage.Undeliverable (undelivered.Reminder, undelivered.Reason));
				return;
			}

			lock (_lockObject) {
				if (!_retryAttempts.ContainsKey (undelivered.ReminderId))
					_retryAttempts.Add (undelivered.ReminderId, 1);
				else
					_retryAttempts [undelivered.ReminderId]++;

				var rescheduled = CalculateNextDueTime (undelivered.Reminder, _retryAttempts[undelivered.ReminderId]);
				if (GiveupRedelivering (rescheduled)) 
					_bus.Send (new ReminderMessage.Undeliverable (undelivered.Reminder, undelivered.Reason));
				 else 
					_bus.Send (rescheduled);
			}
		}
			
		private ReminderMessage.Schedule CalculateNextDueTime(ReminderMessage.Schedule undelivered, int retryCount)
		{
			undelivered.RescheduleFor = undelivered.DueAt.AddTicks (DecelerationFactor (undelivered) * retryCount * retryCount * retryCount);
			return undelivered;
		}

		private long DecelerationFactor(ReminderMessage.Schedule reminder)
		{
			return (long)( (reminder.GiveupAfter.Value.Ticks - reminder.DueAt.Ticks) / (reminder.MaxAttempts * reminder.MaxAttempts * reminder.MaxAttempts) );
		}

		private bool GiveupRedelivering(ReminderMessage.Schedule undelivered)
		{
			return undelivered.MaxAttempts == _retryAttempts[undelivered.ReminderId];
		}
	}
}

