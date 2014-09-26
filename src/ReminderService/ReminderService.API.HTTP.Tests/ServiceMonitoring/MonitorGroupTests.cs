using System;
using NUnit.Framework;
using ReminderService.Messages;
using ReminderService.Common;

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
				new ServiceState.MonitorGroup ((ServiceState.MonitorItem)null));

			Assert.Throws<ArgumentNullException> (() =>
				new ServiceState.MonitorGroup(string.Empty));
		}

		[Test]
		public void Creating_a_monitor_group_from_string()
		{
			Assert.Throws<ArgumentNullException> (() =>
				new ServiceState.MonitorGroup ((ServiceState.MonitorItem)null));

			Assert.Throws<ArgumentNullException> (() =>
				new ServiceState.MonitorGroup(string.Empty));

			ServiceState.MonitorGroup monitorGroup = null;
			Assert.DoesNotThrow (() =>
				monitorGroup = new ServiceState.MonitorGroup("Testing, testing..."));
			Assert.AreEqual ("Testing, testing...", monitorGroup.Name);
			Assert.AreEqual (0, monitorGroup.Items.Count);
			Assert.AreEqual (_now, monitorGroup.TimeStamp);
		}

		[Test]
		public void Creating_a_monitor_group_from_item()
		{
			Assert.Throws<ArgumentNullException> (() =>
				new ServiceState.MonitorGroup ((ServiceState.MonitorItem)null));

			Assert.Throws<ArgumentNullException> (() =>
				new ServiceState.MonitorGroup(string.Empty));

			ServiceState.MonitorGroup monitorGroup = null;
			ServiceState.MonitorItem item = new ServiceState.MonitorItem { 
				Name = "A monitor name",
				TimeStamp = _now,
				Key = "key",
				Value = "value"
			};
			Assert.DoesNotThrow (() =>
				monitorGroup = new ServiceState.MonitorGroup(item));
			Assert.AreEqual (item.Name, monitorGroup.Name);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual (_now, monitorGroup.TimeStamp);
			Assert.AreEqual (item.TimeStamp, monitorGroup.TimeStamp);
		}

		[Test]
		public void Inserting_items()
		{
			var monitorGroup = new ServiceState.MonitorGroup("TestMonitor");
			var item = new ServiceState.MonitorItem { 
				Name = "TestMonitor",
				TimeStamp = _now,
				Key = "key1",
				Value = "value1"
			};
			monitorGroup.AddOrUpdate (item);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual ("value1", monitorGroup.Items[0].Value);

			monitorGroup.AddOrUpdate (item);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual ("value1", monitorGroup.Items[0].Value);

			item.Value = "updated";
			monitorGroup.AddOrUpdate (item);
			Assert.AreEqual (1, monitorGroup.Items.Count);
			Assert.AreEqual ("updated", monitorGroup.Items[0].Value);
		}
	}
}

