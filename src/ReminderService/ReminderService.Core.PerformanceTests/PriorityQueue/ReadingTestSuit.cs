using System;
using System.Collections.Generic;
using ReminderService.Common;
using System.Text;
using System.Linq;
using System.IO;

namespace ReminderService.Core.PerformanceTests.PriorityQueue
{
	public class ReadingTestSuit : ITestSuit
	{
		const string FilePath = "../../Results/AverageReadFromPQ.data";
		const int NumberOfElements = 100000;
		private readonly List<RunableTest> _testsToRun;
		private readonly List<int> _testResults = new List<int> ();

		public ReadingTestSuit ()
		{
			var dueDate = SystemTime.UtcNow ();
			_testsToRun = new List<RunableTest> { 
				new When_reading_from_the_queue(NumberOfElements, dueDate),
				new When_reading_from_the_queue(NumberOfElements, dueDate),
				new When_reading_from_the_queue(NumberOfElements, dueDate),
				new When_reading_from_the_queue(NumberOfElements, dueDate),
				new When_reading_from_the_queue(NumberOfElements, dueDate),
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
			builder.AppendLine ("Average time to read (ms): " + average);
			builder.AppendLine ("Reads per second: " + (NumberOfElements / average) * 1000);

			File.AppendAllText (FilePath, builder.ToString());
		}

		public string Title {
			get {
				return "Reading Large Number of Reminders Due at the Same Time";
			}
		}
	}
}

