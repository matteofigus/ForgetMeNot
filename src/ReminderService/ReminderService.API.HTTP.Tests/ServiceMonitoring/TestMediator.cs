using System;
using System.Reactive.Linq;
using ReminderService.API.HTTP.Monitoring;
using System.Collections;
using System.Collections.Generic;

namespace ReminderService.API.HTTP.Tests.ServiceMonitoring
{
	public class TestMediator : IMediateEvents
	{
		private readonly DateTime now = DateTime.Now;
		private readonly IEnumerable<MonitorEvent> _events;

		public IObservable<MonitorEvent> GetStream {
			get {
				return _events.ToObservable();	
			}
		}

		public TestMediator (IEnumerable<MonitorEvent> events)
		{
			_events = events;
		}
	}
}

