using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace ReminderService.Core.PerformanceTests.PriorityQueue
{
	public interface ITestSuit
	{
		void Run();

		string Title { get; }
	}

	public class InsertPQTestSuit : ITestSuit
	{
		const string FilePath = "../../Results/InsertPriorityQueueTest.data";
		private readonly List<RunableTest> _testsToRun;
		private readonly StringBuilder _testResults = new StringBuilder();

		public string Title {
			get { return "Inserting in to the Priority Queue"; }
		}


		public InsertPQTestSuit ()
		{
			_testsToRun = new List<RunableTest> { 
				new When_writing_to_the_queue(10),
				new When_writing_to_the_queue(100),
				new When_writing_to_the_queue(500),
				new When_writing_to_the_queue(1000),
				new When_writing_to_the_queue(5000),
				new When_writing_to_the_queue(10000),
				new When_writing_to_the_queue(50000),
				new When_writing_to_the_queue(100000),
			};

			WriteColumnHeadings ();
		}

		public void Run ()
		{
			for (int i = 0; i < _testsToRun.Count; i++) {
				_testsToRun [i].Run ();
				UpdateResults (_testsToRun[i].GetResults());
			}

			UpdateFile ();
		}

		private void UpdateResults(Tuple<int, int> result)
		{
			_testResults.AppendLine (string.Format("{0}\t{1}", result.Item1, result.Item2));
		}

		private void UpdateFile()
		{
			File.AppendAllText (FilePath, _testResults.ToString());
		}

		private void WriteColumnHeadings()
		{
			File.WriteAllText (FilePath, "#Time (ms)\tNumber of Elements");
		}
	}
}

