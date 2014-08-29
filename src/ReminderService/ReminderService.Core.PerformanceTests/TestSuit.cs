using System;
using System.Collections.Generic;
using System.Linq;

namespace ReminderService.Core.PerformanceTests
{
	public class TestSuit : RunableTest
	{
		private readonly List<RunableTest> _testsToRun;
		private readonly Dictionary<string, string> _testResults = new Dictionary<string, string>();

		public TestSuit ()
		{
			_testsToRun = new List<RunableTest> { 
				new When_writing_to_the_queue(10),
				new When_writing_to_the_queue(100),
				new When_writing_to_the_queue(1000),
				new When_writing_to_the_queue(10000),
				new When_writing_to_the_queue(100000),
			};
		}

		public void Run ()
		{
			for (int i = 0; i < _testsToRun.Count; i++) {
				_testsToRun [i].Run ();
				_testResults.Add ("Test Run " + i,
					string.Join(", ", 
						_testsToRun [i].GetResults ().Select (r => r.Key + ": " + r.Value)));
			}
		}

		public IDictionary<string, string> GetResults ()
		{
			return _testResults;
		}
	}
}

