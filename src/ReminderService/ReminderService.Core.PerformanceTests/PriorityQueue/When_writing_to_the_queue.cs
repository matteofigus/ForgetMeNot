using System;
using ReminderService.DataStructures;
using ReminderService.Messages;

namespace ReminderService.Core.PerformanceTests
{
	public class When_writing_to_the_queue : RunableTest
	{
		private readonly int _numberOfElements;
		private readonly MinPriorityQueue<ReminderMessage.ISchedulable> _pq;

		public When_writing_to_the_queue (int numberOfElementsToWrite)
		{
			_numberOfElements = numberOfElementsToWrite;
			_pq = new MinPriorityQueue<ReminderMessage.ISchedulable> ((a, b) => a.DueAt > b.DueAt);
		}

		public void Run()
		{
			var dueAt = DateTime.Now.AddDays(1);
			var reminderId = Guid.NewGuid ();

			for (int i = 0; i < _numberOfElements ; i++) {
				_pq.Insert (new TestSchedulable(reminderId, dueAt));
			}
		}

		public object GetResults ()
		{
			throw new NotImplementedException ();
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

