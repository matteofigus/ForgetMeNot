using System;
using System.Diagnostics;

namespace ReminderService.Core.PerformanceTests
{
	public class TestRunner : RunableTest
	{
		private readonly RunableTest _testCase;
		private readonly int n;
		private readonly Stopwatch _stopWatch = new Stopwatch ();

		public TestRunner (int N)
		{
			n = N;
			_testCase = new When_writing_to_the_queue (n);
		}

		public void Run ()
		{
			Console.WriteLine (string.Format("Starting test...with {0} elements...", n));
			_stopWatch.Start ();
			_testCase.Run ();
			_stopWatch.Stop ();

			var elapsed = _stopWatch.ElapsedMilliseconds;
			Console.WriteLine ("Elapsed time: " + elapsed);
			Console.WriteLine ("Elements per ms: " + (elapsed / n));
		}

		public object GetResults ()
		{
			throw new NotImplementedException ();
		}
	}
}

