using System;
using System.Threading;
using ReminderService.DataStructures;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core.ScheduleReminder
{
	public class Scheduler : IConsume<ReminderMessages.ScheduledReminderHasBeenJournaled>, IDisposable
	{
		private readonly IBus _bus;
		private readonly ITimer _timer;
		private readonly MinPriorityQueue<ScheduledReminder> _pq;
		private int _running = 0;

		public Scheduler (IBus bus, ITimer timer)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (timer, "timer");

			_bus = bus;
			_timer = timer;
			_pq = new MinPriorityQueue<ScheduledReminder> ((a, b) => a.TimeOutAt < b.TimeOutAt);
		}
			
		public void Handle (ReminderMessages.ScheduledReminderHasBeenJournaled msg)
		{
			//Add ();
		}

		public void Start()
		{
			Interlocked.Increment (ref _running);
		}

		private void OnTimerFired()
		{
			//get all the items from the pq that are due
			var dueTime = _pq.Min ().TimeOutAt;
			while (!_pq.IsEmpty && _pq.Min ().TimeOutAt <= dueTime) {
				//publish on the bus

			}
		}

		private void SetTimeout()
		{
			if (_running > 0 && !_pq.IsEmpty)
			{
				var nextTimeoutAt = _pq.Min ().TimeOutAt;
				var timeToNext = nextTimeoutAt.Subtract(SystemTime.Now()).Milliseconds;
				Console.WriteLine ("SetTimeout, timeToNext: " + timeToNext);
				_timer.FiresIn (timeToNext, OnTimerFired);
			}
		}

		public void Stop()
		{
			Interlocked.Decrement (ref _running);
		}

		public bool IsRunning
		{
			get { return _running != 0; }
		}

		public void Add(ScheduledReminder reminder)
		{
			//just add to the pq
			//pq will re-order
			//grab the min off the pq and set the timer
			_pq.Insert (reminder);
			SetTimeout ();

			//need to check if this incoming reminder is going to timeout sooner than the current reminder
			//on the top of the queue.
			//if so, we need to add to the queue (so that it is resorted) and then get the new current min
//			if (!_pq.IsEmpty && _pq.Min ().TimeOutAt.Subtract (reminder.TimeOutAt) > TimeSpan.Zero) {
//				Console.WriteLine (
//					string.Format ("Inserting reminder {0} {1:H:mm:ss fff}, cancelling previous", 
//						reminder.ReminderId, reminder.TimeOutAt));
//				_pq.Insert (reminder);
//			}
//			else
//				Console.WriteLine (
//					string.Format("Inserting reminder {0} {1:H:mm:ss fff}", reminder.ReminderId, reminder.TimeOutAt));
//			_pq.Insert (reminder);
//
//			GetNextTimeout ();
		}

		public void Dispose ()
		{
			_timer.Dispose ();
		}
	}
}

