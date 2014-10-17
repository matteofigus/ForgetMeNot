using System;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.API.HTTP.Modules;
using ReminderService.API.HTTP.BootStrap;
using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Test.Common;
using System.Collections.Generic;
using ReminderService.API.HTTP.Models;
using System.Linq;

namespace ReminderService.API.HTTP.Tests.ServiceMonitoring
{
	[TestFixture]
	public class When_getting_service_monitor_state : Given_the_service_is_configured_with_monitoring
	{
		private new BrowserResponse _response;

		[TestFixtureSetUp]
		public void When_some_reminders_have_been_scheduled()
		{
			FreezeTime ();

			var remindersToSchedule = Helpers.BuildScheduleRequests (10);

			foreach (var reminder in remindersToSchedule) {
				POST ("/reminders", reminder);
				Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			}

			_response = GET_ServiceStatus ();
		}

		[Test]
		public void Then_the_service_returns_a_200()
		{
			Assert.AreEqual (HttpStatusCode.OK, _response.StatusCode);
		}

		[Test]
		public void Then_the_response_contains_a_collection_of_monitors()
		{
			var monitors = _response.Body.DeserializeJson<List<MonitorModel.MonitorGroup>> ();
			Assert.NotNull (monitors);
			Assert.AreEqual (2, monitors.Count);
			Assert.IsTrue (monitors.Any(grp => grp.Name == "http:///reminders"));
			Assert.IsTrue (monitors.Any(grp => grp.Name == "Message Stats"));
			Assert.AreEqual (10, monitors.Where (mg => mg.Name == "http:///reminders").SelectMany(mg => mg.Items).Count());
			Assert.AreEqual (4, monitors.Where(mg => mg.Name == "Message Stats").SelectMany(mg => mg.Items).Count());
		}
	}
}

