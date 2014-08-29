using System;
using ReminderService.Core.PerformanceTests;

namespace ReminderService.PerformanceTests.Runner
{
	class MainClass
	{
		private static TestRunner _testRunner = new TestRunner(100000);

		public static void Main (string[] args)
		{
			Console.WriteLine ("Welcome to the test runner...");
			_testRunner.Run ();
		}
	}
}
