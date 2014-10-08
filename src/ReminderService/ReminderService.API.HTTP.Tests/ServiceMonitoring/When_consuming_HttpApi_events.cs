using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using ReminderService.API.HTTP.Monitoring;
using ReminderService.Common;
using System.Collections.Generic;
using ReminderService.API.HTTP.Models;
using ReminderService.Test.Common;

namespace ReminderService.API.HTTP.Tests.ServiceMonitoring
{
	[TestFixture]
	public class When_consuming_HttpApi_events
	{
		const int StreamSize = 200;
		private List<MonitorGroup> _results;

		public When_consuming_HttpApi_events ()
		{
		}

		[TestFixtureSetUp]
		public void Given_some_HttpApi_events()
		{
			var now = SystemTime.FreezeTime ();
			var events = new List<MonitorEvent> {
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (10), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (102), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders GET", now.AddMilliseconds (40), "ResponseTime", 120),
				new MonitorEvent ("route/reminders DELETE", now.AddMilliseconds (50), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (100), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders GET", now.AddMilliseconds (100), "ResponseTime", 120), 
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (100), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders DELETE", now.AddMilliseconds (200), "ResponseTime", 120), 

				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (110), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (1120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (1102), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (1120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders GET", now.AddMilliseconds (410), "ResponseTime", 120),
				new MonitorEvent ("route/reminders DELETE", now.AddMilliseconds (150), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (1100), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (1120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders GET", now.AddMilliseconds (1100), "ResponseTime", 120), 
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (1100), "ResponseTime", 120),
				new MonitorEvent ("route/reminders POST", now.AddMilliseconds (1120), "RequestContentSize", 2300),
				new MonitorEvent ("route/reminders DELETE", now.AddMilliseconds (1200), "ResponseTime", 120), 
			}.ToObservable ();
			var apiMonitor = new HttpApiMonitor (events, 5, 1);
			_results = new List<MonitorGroup> (apiMonitor.GetMonitors ());
		}

		[Test]
		public void Then_3_monitorgroups_are_generated()
		{
			Assert.NotNull (_results);
			Assert.AreEqual (3, _results.Count);
			_results.ContainsThisMany<MonitorGroup> (1, x => x.Name == "route/reminders POST");
			_results.ContainsThisMany<MonitorGroup> (1, x => x.Name == "route/reminders GET");
			_results.ContainsThisMany<MonitorGroup> (1, x => x.Name == "route/reminders DELETE");
		}

		public void TestUsingTestScheduler()
		{
			var scheduler = new TestScheduler ();
			var random = new Random ();

			var httpGetStream = scheduler.CreateHotObservable (
				OnNext(100, new MonitorEvent("route/reminders GET", scheduler.Now.UtcDateTime, "ResponseTime", 120)),
				OnNext(120, new MonitorEvent("route/reminders GET", scheduler.Now.UtcDateTime, "ResponseTime", 120)) 
			);

			var httpPostStream = scheduler.CreateHotObservable (
				OnNext(200, new MonitorEvent("route/reminders POST", scheduler.Now.UtcDateTime, "ResponseTime", 120)),
				OnNext(250, new MonitorEvent("route/reminders POST", scheduler.Now.UtcDateTime, "ResponseTime", 120)) 
			);

			var httpDeleteStream = scheduler.CreateHotObservable (
				OnNext(354, new MonitorEvent("route/reminders DELETE", scheduler.Now.UtcDateTime, "ResponseTime", 120)),
				OnNext(560, new MonitorEvent("route/reminders DELETE", scheduler.Now.UtcDateTime, "ResponseTime", 120))
			);

			var testableObserver = scheduler.Start (
				             () => httpGetStream.Merge (httpPostStream).Merge (httpDeleteStream));
				//create: 0,
				//subscribed: 110,
				//disposed: 1000);



		}

		private Recorded<Notification<MonitorEvent>> OnNext(long time, MonitorEvent evnt)
		{
			return new Recorded<Notification<MonitorEvent>> (time,
				Notification.CreateOnNext (evnt));
		}
	}
}

