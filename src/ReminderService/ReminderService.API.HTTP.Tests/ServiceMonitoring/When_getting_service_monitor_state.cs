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

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class When_getting_service_monitor_state : ServiceSpec<ServiceMonitoringModule>
	{
		[TestFixtureSetUp]
		public void Given_some_reminders_have_been_scheduled()
		{
			FreezeTime ();

			var remindersToSchedule = Helpers.BuildScheduleRequests (10);

			foreach (var reminder in remindersToSchedule) {
				POST ("/reminders", reminder);
				Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			}
		}

		protected BrowserResponse POST(string url, object message)
		{
			return _service.Post(url, with => {
				with.JsonBody(message);
			});
		}



		public When_getting_service_monitor_state ()
		{
//			_busFactory = new BusFactory ()
//				.WithConnectionString(ConnectionString)
//				.WithRestClient (new FakeRestClient())
//				.WithJournaler (new InMemoryJournaler())
//				.WithTimer (_timer);
//
//			_service = new Browser (with => {
//				//with.Module<ServiceMonitoringModule> ();
//				with.Modules(new []{ServiceMonitoringModule, ReminderApiModule});
//				with.Dependency<IBus> (_busFactory.Build ());
//				with.ApplicationStartup ((ioc, pipes) => {
//					ioc.Resolve<IBus> ().Send (new SystemMessage.Start ());
//				});
//				with.EnableAutoRegistration ();
//			});
		}

		protected Browser _service;
		protected IBusFactory _busFactory;
		protected ITimer _timer = new TestTimer();
	}
}

