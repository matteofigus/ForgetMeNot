using System;
using System.Reactive;
using System.Reactive.Linq;
using ReminderService.Router;
using ReminderService.Messages;
using System.Collections.Generic;

namespace ReminderService.Core.ServiceMonitoring
{
	public class HttpApiMonitor
	{
		const int WindowSize = 1000;
		const int WindowSlide = 10;
		private readonly Dictionary<string, MonitorMessage.MonitorGroup> _monitors = new Dictionary<string, MonitorMessage.MonitorGroup>();

		public HttpApiMonitor (ObservableConsumer<MonitorMessage.MonitorEvent> stream)
		{
			stream
				.Window (WindowSize, WindowSlide)
				.Switch ()
				.Aggregate (new WindowState (), WindowState.Calculate)
				.GroupBy (w => w.ComponentName)
				.Switch()
				.SelectMany(window => 
					Observable
						.Return(new MonitorMessage.MonitorItem{
							Topic = window.ComponentName, TimeStamp = window.Last.Value, Key = "First" + window.ItemName, Value = window.First.Value.ToString()})
						.Merge(Observable.Return(new MonitorMessage.MonitorItem{
							Topic = window.ComponentName, TimeStamp = window.Last.Value, Key = "Last" + window.ItemName, Value = window.Last.Value.ToString()}))
						.Merge(Observable.Return(new MonitorMessage.MonitorItem{
							Topic = window.ComponentName, TimeStamp = window.Last.Value, Key = "Min" + window.ItemName, Value = window.Min.ToString()}))
						.Merge(Observable.Return(new MonitorMessage.MonitorItem{
							Topic = window.ComponentName, TimeStamp = window.Last.Value, Key = "Max" + window.ItemName, Value = window.Max.ToString()}))
						.Merge(Observable.Return(new MonitorMessage.MonitorItem{
							Topic = window.ComponentName, TimeStamp = window.Last.Value, Key = "Mean" + window.ItemName, Value = window.Mean.ToString()}))
						.Merge(Observable.Return(new MonitorMessage.MonitorItem{
							Topic = window.ComponentName, TimeStamp = window.Last.Value, Key = "WindowCount", Value = window.WindowCount.ToString()}))
						.Merge(Observable.Return(new MonitorMessage.MonitorItem{
							Topic = window.ComponentName, TimeStamp = window.Last.Value, Key = "WindowDuration", Value = window.WindowDuration.ToString()}))
					)
				.Subscribe (monitor => UpdateMonitors(monitor));
		}

		private void UpdateMonitors(MonitorMessage.MonitorItem item)
		{
			if (!_monitors.ContainsKey (item.Topic))
				_monitors.Add (item.Topic, MonitorMessage.MonitorGroup.Create (item));
			else
				_monitors [item.Topic].Upsert (item);
		}

		public IEnumerable<MonitorMessage.MonitorGroup> GetMonitors()
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

			public static WindowState Calculate(WindowState item, MonitorMessage.MonitorEvent evnt) {

				item.ComponentName = evnt.Topic;
				item.ItemName = evnt.Key;
				item.First = item.First ?? evnt.TimeStamp;
				item.Last = evnt.TimeStamp;
				item.WindowDuration = (item.First.HasValue && item.Last.HasValue) ? item.Last.Value.Subtract (item.First.Value) : TimeSpan.Zero;
				item.WindowCount = item.WindowCount++;
				item.Min = evnt.Value < item.Min ? (int)evnt.Value : item.Min;
				item.Max = evnt.Value > item.Max ? (int)evnt.Value : item.Max;
				item.Sum = item.Sum + (int)evnt.Value;
				item.Mean = item.Sum / item.WindowCount;
				return item;
			}
		}
	}
}

