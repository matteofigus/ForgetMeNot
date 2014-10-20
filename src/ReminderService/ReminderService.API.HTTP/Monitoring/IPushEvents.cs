using System;

namespace ReminderService.API.HTTP.Monitoring
{
	public interface IPushEvents
	{
		void Push(MonitorEvent evnt);
	}
}

