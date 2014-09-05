using System;
using ReminderService.DataStructures;
using ReminderService.Messages;
using System.Collections.Generic;
using System.Diagnostics;
using ReminderService.Common;

namespace ReminderService.Core.PerformanceTests.PriorityQueue
{
	public class When_writing_to_the_queue : RunableTest
	{
		const int TicksPerSecond = 10000000;
		private readonly int _numberOfElements;
		private readonly MinPriorityQueue<ReminderMessage.ISchedulable> _pq;
		private Tuple<int, int> _results;
		private readonly Stopwatch _stopWatch = new Stopwatch();
		private readonly DateTime _now;
		private readonly DateTime _until;
		private readonly Random _random = new Random ();
		private readonly Func<DateTime> _dueDateFactory;

		public When_writing_to_the_queue (int numberOfElementsToWrite)
		{
			_numberOfElements = numberOfElementsToWrite;
			_pq = new MinPriorityQueue<ReminderMessage.ISchedulable> ((a, b) => a.DueAt > b.DueAt);

			//for generating random dates
			_now = SystemTime.UtcNow ();
			_until = _now.AddDays (10);
			_dueDateFactory = () => {
				var range =  (_until - _now).Hours;
				var nextDate = _now.AddHours(_random.Next(range));
				return nextDate;
			};
		}

		public void Run()
		{
			var reminderId = Guid.NewGuid ();

			_stopWatch.Restart ();

			for (int i = 0; i < _numberOfElements ; i++) {
				_pq.Insert (new TestSchedulable(reminderId, _dueDateFactory()));
			}

			_stopWatch.Stop ();

			_results = Tuple.Create((int)_stopWatch.ElapsedMilliseconds, _numberOfElements);
		}

		private long calcUnitsPerSecond(int numberOfElements, long elapsedMilliseconds)
		{
			if (elapsedMilliseconds <= 0)
				return 0;

			var elementsPerMilli = numberOfElements / elapsedMilliseconds;
			var elementsPerSecond = elementsPerMilli * 1000;

			return elementsPerSecond;
		}

		/// <summary>
		/// Time : Number Of Elements
		/// </summary>
		/// <returns>The results.</returns>
		public Tuple<int, int> GetResults ()
		{
			return _results;
		}
	}

	public class TestSchedulable : ReminderMessage.ISchedulable
	{
		public DateTime DueAt {
			get;
			set;
		}

		public Guid ReminderId {
			get;
			set;
		}

		public TestSchedulable (Guid reminderId, DateTime dueAt)
		{
			DueAt = dueAt;
			ReminderId = reminderId;
		}
	}
}

