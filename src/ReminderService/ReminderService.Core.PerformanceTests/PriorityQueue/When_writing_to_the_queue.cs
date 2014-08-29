using System;
using ReminderService.DataStructures;
using ReminderService.Messages;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReminderService.Core.PerformanceTests
{
	public class When_writing_to_the_queue : RunableTest
	{
		private readonly int _numberOfElements;
		private readonly MinPriorityQueue<ReminderMessage.ISchedulable> _pq;
		private readonly Dictionary<string, string> _results = new Dictionary<string, string> ();
		private readonly Stopwatch _stopWatch = new Stopwatch();

		public When_writing_to_the_queue (int numberOfElementsToWrite)
		{
			_numberOfElements = numberOfElementsToWrite;
			_pq = new MinPriorityQueue<ReminderMessage.ISchedulable> ((a, b) => a.DueAt > b.DueAt);
		}

		public void Run()
		{
			var dueAt = DateTime.Now.AddDays(1);
			var reminderId = Guid.NewGuid ();

			_stopWatch.Start ();

			for (int i = 0; i < _numberOfElements ; i++) {
				_pq.Insert (new TestSchedulable(reminderId, dueAt));
			}

			_stopWatch.Stop ();

			_results.Add ("Elapsed Time (ms)", _stopWatch.ElapsedMilliseconds.ToString());
			_results.Add ("Number of elemens inserted", _numberOfElements.ToString());
			//_results.Add ("Elements per ms", (_numberOfElements / (_stopWatch.ElapsedTicks / 10000000)).ToString());
		}

		public IDictionary<string, string> GetResults ()
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

