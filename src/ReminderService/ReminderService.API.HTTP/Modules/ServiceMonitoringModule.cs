using System;
using Nancy;
using ReminderService.Router;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.API.HTTP.Models;
using ReminderService.API.HTTP.Modules;

namespace ReminderService.API.HTTP
{
	public class ServiceMonitoringModule : RootModule
	{
		public ServiceMonitoringModule (IBus bus)
			: base()
		{
			Ensure.NotNull (bus, "bus");

			Get["/service-status"] = parameters => {
				var state = bus.Send(new QueryResponse.GetServiceMonitorState());
				var queueStats = bus.Send(new QueryResponse.GetQueueStats());

				var monitorModel = MonitorGroup
					.Create("Message Stats", SystemTime.UtcNow())
					.AddMonitor(SystemTime.UtcNow(), "UndeliverableCount", state.UndeliverableCount.ToString())
					.AddMonitor(SystemTime.UtcNow(), "QueueSize", queueStats.QueueSize.ToString());

				return Response.AsJson(monitorModel);
			};
		}
	}
}

