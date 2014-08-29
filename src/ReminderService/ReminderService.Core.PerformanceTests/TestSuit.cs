using System;
using System.Collections.Generic;

namespace ReminderService.Core.PerformanceTests
{
	public class TestSuit : RunableTest
	{
		private readonly List<RunableTest> _testsToRun;
		private readonly List<object> _testResults;

		public TestSuit ()
		{
		}

		public void Run ()
		{
			foreach (var test in _testsToRun) {
				test.Run ();
			}
		}

		public object GetResults ()
		{
			throw new NotImplementedException ();
		}
	}
}

