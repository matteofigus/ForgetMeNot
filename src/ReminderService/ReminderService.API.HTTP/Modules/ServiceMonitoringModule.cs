using System;
using Nancy;
using ReminderService.Router;
using ReminderService.Common;
using ReminderService.Messages;

namespace ReminderService.API.HTTP
{
	public class ServiceMonitoringModule : NancyModule
	{
		private readonly IBus _bus;

		public ServiceMonitoringModule (IBus bus)
			: base()
		{
			Ensure.NotNull (bus, "bus");

			_bus = bus;

			Get["/"] = parameters => {
				var state = bus.Send(new QueryResponse.GetServiceMonitorState());
				var queueStats = bus.Send(new QueryResponse.GetQueueStats());
				state.QueueSize = queueStats.QueueSize;

				return Response.AsJson(state);
			};
		}
	}
}

