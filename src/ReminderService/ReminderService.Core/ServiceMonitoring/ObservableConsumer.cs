using System;
using System.Reactive.Subjects;
using ReminderService.Router;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core.ServiceMonitoring
{
	public class ObservableConsumer<T> : IConsume<T>, IObservable<T> where T : IMessage
	{
		private readonly Subject<T> _subject = new Subject<T>();

		//TODO: make connectable observer - maybe we dont need this if we only ever have one connected observer.
		public IDisposable Subscribe (IObserver<T> observer)
		{
			return _subject.Subscribe (observer);
		}

		public void Handle (T msg)
		{
			_subject.OnNext (msg);
		}
	}
}

