using System;
using ReminderService.Core.PerformanceTests.PriorityQueue;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace ReminderService.Core.PerformanceTests.Scheduling
{
	public class SchedulingTestSuit : ITestSuit
	{
		const string FilePath = "../../Results/AverageScheduleRateTest.data";
		const int NumberOfElements = 100000;
		private readonly List<RunableTest> _testsToRun;
		private readonly List<int> _testResults = new List<int> ();

		public SchedulingTestSuit ()
		{
			File.Delete (FilePath);
			_testsToRun = new List<RunableTest> { 
				new When_scheduling_reminders(NumberOfElements),
				new When_scheduling_reminders(NumberOfElements),
				new When_scheduling_reminders(NumberOfElements),
				new When_scheduling_reminders(NumberOfElements),
				new When_scheduling_reminders(NumberOfElements),
			};
		}

		public void Run ()
		{
			foreach (var test in _testsToRun) {
				test.Run ();
				UpdateResults (test.GetResults());
			}

			UpdateFile ();
		}

		private void UpdateResults(Tuple<int, int> result)
		{
			_testResults.Add (result.Item1);
		}

		private void UpdateFile()
		{
			var average = _testResults.Average ();
			var builder = new StringBuilder ();
			builder.AppendLine ("Number of test runs: " + _testResults.Count);
			builder.AppendLine ("Average time to insert (ms): " + average);
			builder.AppendLine ("Inserts per second: " + (NumberOfElements / average) * 1000);

			File.AppendAllText (FilePath, builder.ToString());
		}

		public string Title {
			get {
				return "Scheduling Reminders";
			}
		}
	}
}

