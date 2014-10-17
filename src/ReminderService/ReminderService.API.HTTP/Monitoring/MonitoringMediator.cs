using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace ReminderService.API.HTTP.Monitoring
{
	public class MonitoringMediator : IMediateEvents, IPushEvents
	{
		private readonly Subject<MonitorEvent> _subject = new Subject<MonitorEvent>();

		public IObservable<MonitorEvent> GetStream {
			get { return _subject
					.AsObservable()
					.Publish()
					.RefCount()
					.Repeat(); 
			}
		}

		public void Push(MonitorEvent evnt)
		{
			_subject.OnNext(evnt);
		}
	}
}

