using System;
using Nancy;
using ReminderService.Router;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.API.HTTP.Models;
using ReminderService.API.HTTP.Modules;
using ReminderService.API.HTTP.Monitoring;
using OpenTable.Services.Components.Monitoring.Monitors.HitTracker;

namespace ReminderService.API.HTTP
{
	public class ServiceMonitoringModule : RootModule
	{
		public ServiceMonitoringModule (IBus bus, HitTracker hitTracker)
			: base()
		{
			Ensure.NotNull (bus, "bus");

			Get["/service-status"] = parameters => {
				var state = bus.Send(new QueryResponse.GetServiceMonitorState());
				var queueStats = bus.Send(new QueryResponse.GetQueueStats());
				var monitorFactory = new MonitorFactory(hitTracker);
				var monitors = monitorFactory.Build();

				var messageMonitor = MonitorGroup
					.Create("Message Stats", SystemTime.UtcNow())
					.AddMonitor(SystemTime.UtcNow(), "Undeliverable Count", state.UndeliverableCount.ToString())
					.AddMonitor(SystemTime.UtcNow(), "Delivered Count", state.DeliveredReminderCount.ToString())
					.AddMonitor(SystemTime.UtcNow(), "Service Started At", state.ServiceStartedAt.ToString("O"))
					.AddMonitor(SystemTime.UtcNow(), "QueueSize", queueStats.QueueSize.ToString());

				monitors.Add(messageMonitor);

				return Response.AsJson(monitors);
			};
		}
	}
}

