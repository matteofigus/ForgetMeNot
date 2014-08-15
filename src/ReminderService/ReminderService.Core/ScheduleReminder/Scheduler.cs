using System;
using System.Threading;
using ReminderService.DataStructures;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core.ScheduleReminder
{
	public class Scheduler : IConsume<JournaledEnvelope<ReminderMessage.Schedule>>, 
								IConsume<SystemMessage.Start>, 
								IConsume<SystemMessage.ShutDown>,
								IDisposable
	{
		private readonly object _locker = new object ();
		private readonly IPublish _bus;
		private readonly ITimer _timer;
		private readonly MinPriorityQueue<ReminderMessage.Schedule> _pq;
		private int _running = 0;

		public Scheduler (IPublish bus, ITimer timer)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (timer, "timer");

			_bus = bus;
			_timer = timer;
			_pq = new MinPriorityQueue<ReminderMessage.Schedule> ((a, b) => a.TimeoutAt > b.TimeoutAt);
		}
			
		public void Handle (SystemMessage.Start startMessage)
		{
			Start ();
		}

		public void Handle (SystemMessage.ShutDown stopMessage)
		{
			Stop ();
		}

		public void Handle (JournaledEnvelope<ReminderMessage.Schedule> journaled)
		{
			lock (_locker) {
				_pq.Insert (journaled.Message);
				SetTimeout ();
			}
		}

		private void OnTimerFired()
		{
			//get all the items from the pq that are due
			lock (_locker) {
				while (!_pq.IsEmpty && _pq.Min ().TimeoutAt <= SystemTime.UtcNow()) {
					_bus.Publish (_pq.RemoveMin().AsDue());
				}
			}
		}

		private void SetTimeout()
		{
			if (_running > 0 && !_pq.IsEmpty)
			{
				var nextTimeoutAt = _pq.Min ().TimeoutAt;
				var timeToNext = Convert.ToInt32 (nextTimeoutAt.Subtract (SystemTime.UtcNow ()).TotalMilliseconds); //Int32.MaxValue in milliseconds is about 68 years! Hopefully nobody is going to schedule something that far in the future
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

