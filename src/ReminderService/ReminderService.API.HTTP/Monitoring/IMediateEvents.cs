using System;

namespace ReminderService.API.HTTP.Monitoring
{
	public interface IMediateEvents
	{
		IObservable<MonitorEvent> GetStream { get; }
	}
}

