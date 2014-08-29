using System;

namespace ReminderService.Core.PerformanceTests
{
	public interface RunableTest
	{
		void Run();

		object GetResults();
	}
}

