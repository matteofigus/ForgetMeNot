using System;
using System.Collections.Generic;
using ReminderService.API.HTTP.Models;
using ReminderService.API.HTTP.Monitoring;
using System.Reactive.Linq;

namespace ReminderService.API.HTTP.Monitoring
{
	//todo: maybe split the class in 2; the observable elements of this class from the stateful dictionary
	//that way we can subscribe to the observable from another (or more classes) to project to any form we need.

	public class HttpApiMonitor
	{
		private readonly int _windowSize;
		private readonly int _windowSlide;
		private readonly Dictionary<string, MonitorGroup> _monitors = new Dictionary<string, MonitorGroup>();

		public HttpApiMonitor (IObservable<MonitorEvent> stream, int windowSize, int windowSlide)
		{
			_windowSize = windowSize;
			_windowSlide = windowSlide;

			stream
				.Window(_windowSize, _windowSlide)
				.Switch()
				.GroupBy(w => w.Topic)
				.Subscribe(groupedByTopic => groupedByTopic
					.GroupBy(g => g.Key)
					.Subscribe(groupedByKey => groupedByKey
						.Aggregate(new WindowState (), WindowState.Calculate)
						.SelectMany(window =>
							Observable
							.Return (new MonitorItem {
								Topic = groupedByTopic.Key,
								TimeStamp = window.Last.Value,
								Key = "First" + window.ItemName,
								Value = window.First.Value.ToString ()
							})
							.Concat (Observable.Return (new MonitorItem {
								Topic = groupedByTopic.Key,
								TimeStamp = window.Last.Value,
								Key = "Last" + window.ItemName,
								Value = window.Last.Value.ToString ()
							}))
							.Concat (Observable.Return (new MonitorItem {
								Topic = groupedByTopic.Key,
								TimeStamp = window.Last.Value,
								Key = "Min" + window.ItemName,
								Value = window.Min.ToString ()
							}))
							.Concat (Observable.Return (new MonitorItem {
								Topic = groupedByTopic.Key,
								TimeStamp = window.Last.Value,
								Key = "Max" + window.ItemName,
								Value = window.Max.ToString ()
							}))
							.Concat (Observable.Return (new MonitorItem {
								Topic = groupedByTopic.Key,
								TimeStamp = window.Last.Value,
								Key = "Mean" + window.ItemName,
								Value = window.Mean.ToString ()
							}))
							.Concat (Observable.Return (new MonitorItem {
								Topic = groupedByTopic.Key,
								TimeStamp = window.Last.Value,
								Key = "WindowCount",
								Value = window.WindowCount.ToString ()
							}))
							.Concat (Observable.Return (new MonitorItem {
								Topic = groupedByTopic.Key,
								TimeStamp = window.Last.Value,
								Key = "WindowDurationMs",
								Value = window.WindowDuration.TotalMilliseconds.ToString ()
							}))
						)
						.Subscribe (monitor => UpdateMonitors (monitor))
					)
				);
		}

		private void UpdateMonitors(MonitorItem item)
		{
			if (!_monitors.ContainsKey (item.Topic))
				_monitors.Add (item.Topic, MonitorGroup.Create (item));
			else
				_monitors [item.Topic].Upsert (item);
		}

		public IEnumerable<MonitorGroup> GetMonitors()
		{
			return _monitors.Values;
		}

		private class WindowState
		{
			public string ComponentName { get; set; }
			public string ItemName { get; set; }
			public DateTime? First { get; set; }
			public DateTime? Last { get; set; }
			public TimeSpan WindowDuration { get; set; }
			public int WindowCount { get; set; }
			public int Min { get; set; }
			public int Max { get; set; }
			public int Sum { get; set; }
			public int Mean { get; set; }

			public static WindowState Calculate(WindowState item, MonitorEvent evnt) {

				item.ComponentName = evnt.Topic;
				item.ItemName = evnt.Key;
				item.First = item.First ?? evnt.TimeStamp;
				item.Last = evnt.TimeStamp;
				item.WindowDuration = (item.First.HasValue && item.Last.HasValue) ? item.Last.Value.Subtract (item.First.Value) : TimeSpan.Zero;
				item.WindowCount = item.WindowCount + 1;
				item.Min = evnt.Value < item.Min || item.Min == 0 ? (int)evnt.Value : item.Min;
				item.Max = evnt.Value > item.Max ? (int)evnt.Value : item.Max;
				item.Sum = item.Sum + (int)evnt.Value;
				item.Mean = item.WindowCount > 0 ? item.Sum / item.WindowCount : 0;
				return item;
			}

			public override string ToString ()
			{
				return string.Format ("[WindowState: ComponentName={0}, ItemName={1}, First={2}, Last={3}, WindowDuration={4}, WindowCount={5}, Min={6}, Max={7}, Sum={8}, Mean={9}]", ComponentName, ItemName, First, Last, WindowDuration, WindowCount, Min, Max, Sum, Mean);
			}
		}
	}
}

