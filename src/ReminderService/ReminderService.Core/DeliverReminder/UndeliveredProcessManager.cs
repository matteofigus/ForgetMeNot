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

				var retryAttempts = _retryAttempts [undelivered.ReminderId];
				var nextDueTime = CalculateNextDueTime (undelivered.Reminder, retryAttempts);
				var rescheduled = new ReminderMessage.Rescheduled (undelivered.Reminder, nextDueTime);

				if (GiveupRedelivering (rescheduled, retryAttempts)) 
					_bus.Send (new ReminderMessage.Undeliverable (undelivered.Reminder, undelivered.Reason));
				 else 
					_bus.Send (rescheduled);
			}
		}
			
		private DateTime CalculateNextDueTime(ReminderMessage.Schedule reminder, int retryCount)
		{
			return reminder.DueAt.AddTicks (DecelerationFactor (reminder) * retryCount * retryCount * retryCount);
		}

		private long DecelerationFactor(ReminderMessage.Schedule reminder)
		{
			return (long)( (reminder.GiveupAfter.Value.Ticks - reminder.DueAt.Ticks) / (reminder.MaxRetries * reminder.MaxRetries * reminder.MaxRetries) );
		}

		private bool GiveupRedelivering(ReminderMessage.Rescheduled rescheduled, int retryAttempts)
		{
			return retryAttempts > rescheduled.Reminder.MaxRetries;
		}
	}
}

