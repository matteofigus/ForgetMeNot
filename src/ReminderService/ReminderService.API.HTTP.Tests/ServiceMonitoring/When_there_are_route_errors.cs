using System;
using OpenTable.Services.Components.Monitoring.Monitors.HitTracker;
using ReminderService.Common;
using NUnit.Framework;
using System.Linq;
using ReminderService.API.HTTP.Models;
using System.Collections.Generic;

namespace ReminderService.API.HTTP.Tests.ServiceMonitoring
{
	[TestFixture]
	public class When_there_are_route_errors
	{
		private HitTracker _hitTracker;
		private DateTime _now;
		private List<MonitorGroup> _groups;

		[TestFixtureSetUp]
		public void Given_no_failed_hits_have_been_received()
		{
			_now = SystemTime.UtcNow ();
			var settings = HitTrackerSettings.Instance;
			settings.DefaultHistoryLimit = 10;
			settings.DefaultErrorThreshold = 0;

			_hitTracker = new HitTracker(settings);

			_hitTracker.AppendHit ("/route/1", new Hit{IsError = true, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = true, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/2", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
		}

		[SetUp]
		public void When_MonitorGroups_are_built()
		{
			var factory = new MonitorFactory (_hitTracker);
			_groups =  factory.Build ().ToList<MonitorGroup>();
			Assert.IsNotNull (_groups);
		}

		[Test]
		public void Then_route1_should_report_failed()
		{
			var route1 = _groups.First (mg => mg.Name == "/route/1");
			Assert.NotNull (route1);
			Assert.AreEqual (10, route1.Items.Count);
			Assert.AreEqual ("2", route1.Items.First(i => i.Key == "ErrorCount").Value);
		}

		[Test]
		public void Then_route2_should_report_NOT_failed()
		{
			var route2 = _groups.First (mg => mg.Name == "/route/2");
			Assert.NotNull (route2);
			Assert.AreEqual (10, route2.Items.Count);
			Assert.AreEqual ("0", route2.Items.First(i => i.Key == "ErrorCount").Value);
		}
	}
}

