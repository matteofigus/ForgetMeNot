using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ReminderService.PerformanceTests.Runner
{
	public class FileWriter
	{
		private string _fileName;

		public FileWriter (string fileName)
		{
			_fileName = fileName;
		}

		public void WriteResultsToFile(List<Dictionary<string, List<string>>> results)
		{
			foreach (var suit in results) {
				WriteColumnHeadings (suit.Keys);
				var numberOfColumns = suit.Keys.Count;
				var numberOfRows = suit[suit.Keys.First()].Count;
				var columns = suit.Keys.ToList ();
				for (int row = 0; row < numberOfRows-1; row++) {
				//foreach(var row in suit) {
					var lineBuilder = new StringBuilder ();
					//for (int col = 0; col < numberOfColumns -1; col++) {
					foreach(var col in columns) {
						lineBuilder.AppendFormat ("{0},", suit[col][row]);
					}
					File.AppendAllText (_fileName, lineBuilder.ToString());
				}
//				var lines = suit.Select(i => string.Format("{0}, {1}", i.Key.ToString(), i.Value.ToString()));
//				File.WriteAllLines (_fileName, lines);
			}
		}

		private void WriteColumnHeadings(IEnumerable<string> headings)
		{
			var line = string.Join (", ", headings);
			File.WriteAllText (_fileName, line);
		}
	}
}

