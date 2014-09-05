using System;
using ReminderService.Core.PerformanceTests;
using System.Collections.Generic;
using ReminderService.Core.PerformanceTests.PriorityQueue;
using ReminderService.Core.PerformanceTests.Scheduling;

namespace ReminderService.PerformanceTests.Runner
{
	class MainClass
	{
		private static List<ITestSuit> _tests = new List<ITestSuit>{
			new InsertPQTestSuit(),
			new AvergeTimeToInsert(),
			new ReadingTestSuit(),
			new SchedulingTestSuit(),
		};

		public static void Main (string[] args)
		{
			Console.WriteLine ("Welcome to the test runner...");

			foreach (var test in _tests) {
				Console.WriteLine ("Running test: " + test.Title);
				test.Run ();
				Console.WriteLine ("Run complete.");
			}

			Console.WriteLine ("Finished test runs.");
		}
	}
}
