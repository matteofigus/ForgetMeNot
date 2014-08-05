using System;
using System.Reactive.Linq;
using System.Reactive;

namespace ReminderService.Router
{
	public static class Extensions
	{
		public static void AsObserverOf<T>(this IBus bus, IObservable<T> observable)
		{
			//observable.sub.Subscribe(Observer.Create((msg) => bus.Subscribe<T>((IConsume<T>)null)));
		}
	}
}

