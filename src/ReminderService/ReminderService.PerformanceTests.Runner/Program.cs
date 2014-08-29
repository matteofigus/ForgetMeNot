using System;
using ReminderService.Core.PerformanceTests;

namespace ReminderService.PerformanceTests.Runner
{
	class MainClass
	{
		private static TestSuit _testRunner = new TestSuit();

		public static void Main (string[] args)
		{
			Console.WriteLine ("Welcome to the test runner...");
			_testRunner.Run ();
			Console.WriteLine ("Results:");
			foreach (var result in _testRunner.GetResults()) {
				Console.WriteLine (result);
			}

		}
	}
}
