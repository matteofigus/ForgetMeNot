using System;
using System.Collections.Generic;

namespace ReminderService.API.HTTP.Tests
{
	public static class MonitorModel
	{
		public class MonitorItem
		{
			public string Topic { get; set; }
			public DateTime TimeStamp { get; set; }
			public string Key { get; set; }
			public string Value { get; set; }
		}

		public class MonitorGroup
		{
			public string Name { get; set; }
			public DateTime TimeStamp { get; set; }
			public List<MonitorItem> Items { get; set; }
		}
	}
}

