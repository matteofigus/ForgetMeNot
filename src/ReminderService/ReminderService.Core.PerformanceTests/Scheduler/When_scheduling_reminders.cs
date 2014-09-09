using System;
using System.Diagnostics;
using ReminderService.Common;
using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Core.Tests;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Test.Common;

namespace ReminderService.Core.PerformanceTests.Scheduling
{
	public class When_scheduling_reminders : RunableTest
	{
		private readonly Scheduler _scheduler;
		private readonly IBus _fakeBus;
		private readonly ITimer _testTimer;
		private readonly Stopwatch _stopwatch = new Stopwatch();
		private readonly int _numberOfRequests;
		private Tuple<int, int> _results;

		public When_scheduling_reminders (int numberOfRequests)
		{
			_fakeBus = new FakeBus ();
			_testTimer = new TestTimer ();
			_scheduler = new Scheduler (_fakeBus, _testTimer);
			_numberOfRequests = numberOfRequests;
		}

		public void Run()
		{
			_stopwatch.Start ();

			for(int i = 0; i < _numberOfRequests; i++){
				_scheduler.Handle (new Envelopes.Journaled<ReminderMessage.Schedule>(
						new ReminderMessage.Schedule(
						SystemTime.UtcNow(),
						"deliveryurl",
						"text",
						"utf8",
						ReminderMessage.TransportEnum.http, 
						new byte[0],
						0
					)
				));
			}

			_stopwatch.Stop();

			_results = Tuple.Create((int)_stopwatch.ElapsedMilliseconds, _numberOfRequests);
		}

		public Tuple<int, int> GetResults ()
		{
			return _results;
		}
	}
}

