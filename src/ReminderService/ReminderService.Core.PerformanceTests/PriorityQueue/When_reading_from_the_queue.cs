using System;
using ReminderService.DataStructures;
using ReminderService.Messages;
using System.Diagnostics;

namespace ReminderService.Core.PerformanceTests.PriorityQueue
{
	public class When_reading_from_the_queue : RunableTest
	{
		const int TicksPerSecond = 10000000;
		private readonly int _queueCapacity;
		private readonly MinPriorityQueue<ReminderMessage.ISchedulable> _pq;
		private Tuple<int, int> _results;
		private readonly Stopwatch _stopWatch = new Stopwatch();
		private readonly DateTime _dueDate;

		public When_reading_from_the_queue (int queueCapacity, DateTime dueDate)
		{
			_queueCapacity = queueCapacity;
			_pq = new MinPriorityQueue<ReminderMessage.ISchedulable> ((a, b) => a.DueAt > b.DueAt);
			_dueDate = dueDate;
		}

		public void Run()
		{
			var reminderId = Guid.NewGuid ();

			for (int i = 0; i < _queueCapacity ; i++) {
				_pq.Insert (new TestSchedulable(reminderId, _dueDate));
			}
				
			_stopWatch.Restart ();

			while(!_pq.IsEmpty) {
				_pq.RemoveMin ();
			}

			_stopWatch.Stop ();

			_results = Tuple.Create((int)_stopWatch.ElapsedMilliseconds, _queueCapacity);
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
}

