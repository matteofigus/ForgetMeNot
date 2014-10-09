using System;
using Nancy;
using ReminderService.Router;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.API.HTTP.Models;
using ReminderService.API.HTTP.Modules;
using ReminderService.API.HTTP.Monitoring;

namespace ReminderService.API.HTTP
{
	public class ServiceMonitoringModule : RootModule
	{
		public ServiceMonitoringModule (IBus bus, HttpApiMonitor httpMonitor)
			: base()
		{
			Ensure.NotNull (bus, "bus");

			Get["/service-status"] = parameters => {
				var state = bus.Send(new QueryResponse.GetServiceMonitorState());
				var queueStats = bus.Send(new QueryResponse.GetQueueStats());
				var monitors = httpMonitor.GetMonitors();

				var messageMonitor = MonitorGroup
					.Create("Message Stats", SystemTime.UtcNow())
					.AddMonitor(SystemTime.UtcNow(), "UndeliverableCount", state.UndeliverableCount.ToString())
					.AddMonitor(SystemTime.UtcNow(), "QueueSize", queueStats.QueueSize.ToString());

				monitors.Add(messageMonitor);

				return Response.AsJson(monitors);
			};
		}
	}
}

