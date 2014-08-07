using NUnit.Framework;
using System;
using System.Collections.Generic;
using Nancy.Testing;
using Nancy;
using RestSharp;
using ReminderService.Test.Common;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;
using ReminderService.Core;
using ReminderService.Core.Persistence.Postgres;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public abstract class ServiceSpec<TModule> where TModule : NancyModule
	{
		const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";
		private Browser _service;
		private FakeRestClient _restClient;
		private IRestResponse _restResponse = new RestResponse{ StatusCode = System.Net.HttpStatusCode.Created, ResponseStatus = ResponseStatus.Completed };
		private TestTimer _timer = new TestTimer();
		private BrowserResponse _response;
		//private InMemoryJournaler _journaler;
		private PostgresJournaler _journaler;

		[TestFixtureSetUp]
		public void BeforeAll()
		{
			_restClient = new FakeRestClient (new []{ _restResponse });
			_journaler = new PostgresJournaler (new PostgresCommandFactory (), ConnectionString);
			//_journaler = new InMemoryJournaler ();

			var busFactory = new BusFactory ()
				.WithConnectionString(ConnectionString)
				.WithRestClient (_restClient)
				.WithJournaler (_journaler)
				.WithTimer (_timer);

			_service = new Browser (with => {
				with.Module<TModule> ();
				with.Dependency<IBus> (busFactory.Build ());
				with.ApplicationStartup ((ioc, pipes) => {
					ioc.Resolve<IBus> ().Publish (new SystemMessage.Start ());
				});
				with.EnableAutoRegistration ();
			});
		}

		protected BrowserResponse Response {
			get { return _response; }
		}

		protected void POST(string url, IMessage message)
		{
			_response = _service.Post(url, with => {
				with.JsonBody(message);
			});
		}

		protected void DELETE(string url, Guid reminderId)
		{
			_response = _service.Delete(url + "/" + reminderId.ToString());
		}

		protected void SetHttpClientResponse(IRestResponse response)
		{
			_restResponse = response;
		}

		protected IRestRequest DeliveryRequest{
			get { return _restClient.LastRequest; }
		}

//		protected IList<IMessage> JournaledMessages{
//			get { return _journaler.JournaledMessages; }
//		}

		protected DateTime Now{
			get { return SystemTime.Now (); }
		}

		protected void FreezeTime()
		{
			SystemTime.FreezeTime ();
		}

		protected void AdvanceTimeBy(TimeSpan timespan)
		{
			SystemTime.Set (Now.Add(timespan));
		}

		protected void FireScheduler()
		{
			_timer.Fire ();
		}
	}
}

