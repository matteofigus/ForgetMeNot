using System;
using System.Threading;
using ReminderService.DataStructures;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;
using log4net;

namespace ReminderService.Core.ScheduleReminder
{
	public class Scheduler : 
		IConsume<Envelopes.Journaled<ReminderMessage.Schedule>>, 
		IConsume<ReminderMessage.Rescheduled>,
		IConsume<SystemMessage.Start>, 
		IConsume<SystemMessage.ShutDown>,
		IDisposable
	{
		private readonly static ILog Logger = LogManager.GetLogger(typeof(Scheduler));
		private readonly object _locker = new object ();
		private readonly ISendMessages _bus;
		private readonly ITimer _timer;
		private readonly MinPriorityQueue<ReminderMessage.ISchedulable> _pq;
		private int _running = 0;

		public Scheduler (ISendMessages bus, ITimer timer)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (timer, "timer");

			_bus = bus;
			_timer = timer;
			_pq = new MinPriorityQueue<ReminderMessage.ISchedulable> ((a, b) => a.DueAt > b.DueAt);
		}
			
		public void Handle (SystemMessage.Start startMessage)
		{
			Start ();
		}

		public void Handle (SystemMessage.ShutDown stopMessage)
		{
			Stop ();
		}

		public void Handle (Envelopes.Journaled<ReminderMessage.Schedule> journaled)
		{
			Logger.DebugFormat ("Scheduling reminder [{0}]", journaled.Message.ReminderId);
			lock (_locker) {
				_pq.Insert (journaled.Message);
				SetTimeout ();
			}
		}

		public void Handle(ReminderMessage.Rescheduled rescheduled)
		{
			Logger.DebugFormat ("Re-scheduling reminder [{0}]", rescheduled.ReminderId);
			lock (_locker) {
				_pq.Insert (rescheduled);
				SetTimeout ();
			}
		}

		private void OnTimerFired()
		{
			//get all the items from the pq that are due
			lock (_locker) {
				while (!_pq.IsEmpty && _pq.Min ().DueAt <= SystemTime.UtcNow()) {
					var due = _pq.RemoveMin ().AsDue ();
					Logger.DebugFormat ("Timer fired. Dequeing reminder {0}", due.ReminderId);
					_bus.Send (due);
				}
			}
		}

		private void SetTimeout()
		{
			if (_running > 0 && !_pq.IsEmpty)
			{
				var nextTimeoutAt = _pq.Min ().DueAt;
				var timeToNext = Convert.ToInt32 (nextTimeoutAt.Subtract (SystemTime.UtcNow ()).TotalMilliseconds); //Int32.MaxValue in milliseconds is about 68 years! Hopefully nobody is going to schedule something that far in the future
				Logger.DebugFormat ("Setting next timeout for {0}ms", timeToNext);
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

