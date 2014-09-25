using System;
using NUnit.Framework;
using ReminderService.Messages;
using ReminderService.Core.ReadModels;

namespace ReminderService.Core.Tests.ReadModels
{
	[TestFixture]
	public class A_ServiceMonitor_Model
	{
		private ServiceMonitor _serviceMonitor;
		private QueryResponse.ServiceMonitorState _monitorState;

		[TestFixtureSetUp]
		public void Given_a_bunch_of_messages ()
		{
			_serviceMonitor = new ServiceMonitor ();
			_serviceMonitor.Handle (new ReminderMessage.Delivered(Guid.NewGuid(), DateTime.Now));
			_serviceMonitor.Handle (new ReminderMessage.Delivered(Guid.NewGuid(), DateTime.Now));
			_serviceMonitor.Handle (new ReminderMessage.Undelivered(new ReminderMessage.Schedule{ReminderId = Guid.NewGuid()}, "a reason"));
			_serviceMonitor.Handle (new ReminderMessage.Undeliverable(new ReminderMessage.Schedule{ReminderId = Guid.NewGuid()}, "a reason"));

			_monitorState = _serviceMonitor.Handle (new QueryResponse.GetServiceMonitorState());
		}

		[Test]
		public void Should_track_UndeliveredReminders()
		{
			Assert.AreEqual (1, _monitorState.UndeliveredCount);
		}

		[Test]
		public void Should_track_DeliveredReminders()
		{
			Assert.AreEqual (2, _monitorState.DeliveredReminderCount);
		}

		[Test]
		public void Should_track_UndeliverableReminders()
		{
			Assert.AreEqual (1, _monitorState.UndeliverableCount);
		}

		[Test]
		public void Should_track_QueueSize()
		{

		}
	}
}

