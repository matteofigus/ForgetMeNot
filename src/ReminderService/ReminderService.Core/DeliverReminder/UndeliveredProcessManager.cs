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
		private readonly Dictionary<Guid, ReminderMessage.Undelivered> _deliveryStates = new Dictionary<Guid, ReminderMessage.Undelivered>();

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

			//if the reminder does not exist in the collection:
			//figure out the first retry time
			//insert in to the collection
			//send on the bus

			//if the reminder does exist in the collection:
			//are we at the GiveupAfter time -> send to deadletter queue
			//figure out the next retry time
			//send on the bus

			lock (_lockObject) {
				if (!_deliveryStates.ContainsKey (undelivered.ReminderId)) {
					_deliveryStates.Add (undelivered.ReminderId, undelivered);
				}

				var rescheduled = CalculateNextDueTime (undelivered.Reminder);
				if (GiveupRedelivering (rescheduled)) 
					_bus.Send (new ReminderMessage.Undeliverable (undelivered.Reminder, undelivered.Reason));
				 else 
					_bus.Send (rescheduled);
			}
		}

		private ReminderMessage.Schedule CalculateNextDueTime(ReminderMessage.Schedule undelivered)
		{
			if (!undelivered.RescheduleFor.HasValue)
				undelivered.RescheduleFor = undelivered.DueAt.AddMilliseconds (undelivered.FirstWaitDurationMs);
			else {
				var nextSchedule = new DateTime (((undelivered.RescheduleFor.Value.Ticks - undelivered.DueAt.Ticks) * 2) + undelivered.RescheduleFor.Value.Ticks);
				undelivered.RescheduleFor = nextSchedule;
			}

			return undelivered;
		}

		private bool GiveupRedelivering(ReminderMessage.Schedule undelivered)
		{
			return undelivered.RescheduleFor.Value > undelivered.GiveupAfter.Value;
		}
	}
}

