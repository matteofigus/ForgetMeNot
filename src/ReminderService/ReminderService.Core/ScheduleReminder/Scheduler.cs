using System;
using System.Threading;
using ReminderService.DataStructures;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core.ScheduleReminder
{
	public class Scheduler : IConsume<ReminderMessage.ScheduledHasBeenJournaled>, 
								IConsume<SystemMessage.Start>, 
								IConsume<SystemMessage.ShutDown>,
								IDisposable
	{
		private readonly object _locker = new object ();
		private readonly IPublish _bus;
		private readonly ITimer _timer;
		private readonly MinPriorityQueue<ReminderMessage.ScheduledHasBeenJournaled> _pq;
		private int _running = 0;

		public Scheduler (IPublish bus, ITimer timer)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (timer, "timer");

			_bus = bus;
			_timer = timer;
			_pq = new MinPriorityQueue<ReminderMessage.ScheduledHasBeenJournaled> ((a, b) => a.Reminder.TimeoutAt > b.Reminder.TimeoutAt);
		}
			
		public void Handle (SystemMessage.Start startMessage)
		{
			Start ();
		}

		public void Handle (SystemMessage.ShutDown stopMessage)
		{
			Stop ();
		}

		public void Handle (ReminderMessage.ScheduledHasBeenJournaled reminder)
		{
			lock (_locker) {
				_pq.Insert (reminder);
				SetTimeout ();
			}
		}

		private void OnTimerFired()
		{
			//get all the items from the pq that are due
			lock (_locker) {
				while (!_pq.IsEmpty && _pq.Min ().Reminder.TimeoutAt <= SystemTime.Now()) {
					//IMessage dueReminder = _pq.RemoveMin ();
					_bus.Publish (_pq.RemoveMin()); //TODO: do we want to have an Action<T> that we invoke here?
				}
			}
		}

		private void SetTimeout()
		{
			if (_running > 0 && !_pq.IsEmpty)
			{
				var nextTimeoutAt = _pq.Min ().Reminder.TimeoutAt;
				var timeToNext = nextTimeoutAt.Subtract(SystemTime.Now()).Milliseconds;
				Console.WriteLine ("SetTimeout, timeToNext: " + timeToNext);
				_timer.FiresIn (timeToNext, OnTimerFired);
			}
		}

		public void Start()
		{
			Interlocked.CompareExchange (ref _running, 1, 0);
		}

		public void Stop()
		{
			Interlocked.Decrement (ref _running);
		}

		public bool IsRunning
		{
			get { return _running != 0; }
		}

		public void Dispose ()
		{
			_timer.Dispose ();
		}
	}
}

