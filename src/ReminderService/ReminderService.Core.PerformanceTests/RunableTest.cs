using System;
using System.Collections.Generic;

namespace ReminderService.Core.PerformanceTests
{
	public interface RunableTest
	{
		void Run();

		Tuple<int, int> GetResults();
	}
}

