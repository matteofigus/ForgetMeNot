using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReminderService.API.HTTP.Monitoring;
using System.Linq;
using OpenTable.Services.Components.Monitoring.Monitors.HitTracker;
using ReminderService.Common;

namespace ReminderService.API.HTTP.Tests.ServiceMonitoring
{
	[TestFixture]
	public class When_there_are_no_route_errors
	{
		private HitTracker _hitTracker;
		private DateTime _now;

		[TestFixtureSetUp]
		public void Given_no_failed_hits_have_been_received()
		{
			_now = SystemTime.UtcNow ();
			var settings = HitTrackerSettings.Instance;
			settings.DefaultHistoryLimit = 10;
			settings.DefaultErrorThreshold = 0;

			_hitTracker = new HitTracker(settings);
			_hitTracker.Clear ();

			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
			_hitTracker.AppendHit ("/route/1", new Hit{IsError = false, StartTime = _now.AddSeconds(100), TimeTaken = TimeSpan.FromMilliseconds(100) });
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
		}

		[Test]
		public void Builds_a_MonitorGroup()
		{
			var factory = new MonitorFactory (_hitTracker);
			var monitorGroup = factory.Build ().ToList();
			Assert.IsNotNull (monitorGroup);
			Assert.AreEqual (2, monitorGroup.Count());
			Assert.IsTrue (monitorGroup.Select(g => g.Name == "/route/1").Any());
			Assert.IsTrue (monitorGroup.Select(g => g.Name == "/route/2").Any());
			Assert.AreEqual (10, monitorGroup.First ().Items.Count);
			Assert.AreEqual (10, monitorGroup.Last().Items.Count);
		}
	}
}

