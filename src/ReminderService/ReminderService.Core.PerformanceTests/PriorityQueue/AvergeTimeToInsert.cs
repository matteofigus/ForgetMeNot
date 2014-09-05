using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ReminderService.Common;

namespace ReminderService.Core.PerformanceTests.PriorityQueue
{
	public class AvergeTimeToInsert : ITestSuit
	{
		const string FilePath = "../../Results/AverageInsertTest.data";
		const int NumberOfElements = 100000;
		private readonly List<RunableTest> _testsToRun;
		private readonly List<int> _testResults = new List<int> ();

		public AvergeTimeToInsert ()
		{
			File.Delete (FilePath);
			_testsToRun = new List<RunableTest> { 
				new When_writing_to_the_queue(NumberOfElements),
				new When_writing_to_the_queue(NumberOfElements),
				new When_writing_to_the_queue(NumberOfElements),
				new When_writing_to_the_queue(NumberOfElements),
				new When_writing_to_the_queue(NumberOfElements),
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
				return "Average Time to Insert in to the Priority Queue";
			}
		}
	}
}

