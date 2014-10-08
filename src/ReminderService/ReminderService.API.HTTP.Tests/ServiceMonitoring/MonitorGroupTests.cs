using System;
using NUnit.Framework;
using ReminderService.Messages;
using ReminderService.Common;
using ReminderService.API.HTTP.Models;

namespace ReminderService.API.HTTP.Tests.ServiceMonitoring
{
	[TestFixture]
	public class MonitorGroupTests
	{
		private DateTime _now;

		[TestFixtureSetUp]
		public void FreezeTime()
		{
			_now = SystemTime.FreezeTime ().ToUniversalTime();
		}

		[Test]
		public void Creating_a_monitor_group_throws()
		{
			Assert.Throws<ArgumentNullException> (() =>
				new MonitorGroup(string.Empty, DateTime.Now));
		}

		[Test]
		public void Creating_a_monitor_group_from_string()
		{
			Assert.Throws<ArgumentNullException> (() =>
				new MonitorGroup (string.Empty, DateTime.MinValue));

			Assert.Throws<ArgumentNullException> (() =>
				new MonitorGroup(string.Empty, DateTime.Now));

			MonitorGroup monitorGroup = null;
			Assert.DoesNotThrow (() =>
				monitorGroup = new MonitorGroup("Testing, testing...", _now));
			Assert.AreEqual ("Testing, testing...", monitorGroup.Name);
			Assert.AreEqual (0, monitorGroup.Items.Count);
			Assert.AreEqual (_now, monitorGroup.TimeStamp);
		}

		[Test]
		public void Creating_a_monitor_group_from_item()
		{
			Assert.Throws<ArgumentNullException> (() =>
				new MonitorGroup(string.Empty, DateTime.Now));

			MonitorGroup monitorGroup = null;
			MonitorItem item = new MonitorItem { 
				TimeStamp = _now,
				Key = "key",
				Value = "value"
			};
			Assert.DoesNotThrow (() =>
				monitorGroup = new MonitorGroup(item));
			Assert.AreEqual ("name", monitorGroup.Name);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual (_now, monitorGroup.TimeStamp);
			Assert.AreEqual (item.TimeStamp, monitorGroup.TimeStamp);
		}

		[Test]
		public void Inserting_items()
		{
			var monitorGroup = new MonitorGroup("TestMonitor", _now);
			var item = new MonitorItem { 
				TimeStamp = _now,
				Key = "key1",
				Value = "value1"
			};
			monitorGroup.Upsert (item);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual ("value1", monitorGroup.Items[0].Value);

			monitorGroup.Upsert (item);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual ("value1", monitorGroup.Items[0].Value);

			item.Value = "updated";
			monitorGroup.Upsert (item);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual ("updated", monitorGroup.Items[0].Value);
		}

		[Test]
		public void Extension_method_adds_item()
		{
			var monitorGroup = new MonitorGroup("TestMonitor", _now);
			var groupWithItem = monitorGroup.AddMonitor (_now, "newkey", "newvalue");

			Assert.AreEqual (1, groupWithItem.Items.Count);
			Assert.AreEqual ("newkey", monitorGroup.Items[0].Key);
			Assert.AreEqual ("newvalue", monitorGroup.Items[0].Value);
		}

		[Test]
		public void Extension_method_updates_item()
		{
			var monitorGroup = new MonitorGroup("TestMonitor", _now);
			var groupWithItem = monitorGroup.AddMonitor (_now, "newkey", "11");

			Assert.AreEqual (1, groupWithItem.Items.Count);
			Assert.AreEqual ("newkey", monitorGroup.Items[0].Key);
			Assert.AreEqual ("11", monitorGroup.Items[0].Value);

			monitorGroup.Update ("newkey", 12.ToString());

			Assert.AreEqual (1, groupWithItem.Items.Count);
			Assert.AreEqual ("newkey", monitorGroup.Items[0].Key);
			Assert.AreEqual ("12", monitorGroup.Items[0].Value);
		}
	}
}

