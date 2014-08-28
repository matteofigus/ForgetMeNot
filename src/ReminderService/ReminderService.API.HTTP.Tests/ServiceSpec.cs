using System;
using System.Collections.Generic;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using ReminderService.API.HTTP.BootStrap;
using ReminderService.Common;
using ReminderService.Core;
using ReminderService.Core.Persistence.Postgres;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Test.Common;
using RestSharp;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public abstract class ServiceSpec<TModule> : PostgresTestBase where TModule : NancyModule
	{
		const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";
		private Browser _service;
		private IBusFactory _busFactory;
		private FakeRestClient _restClient;
		private IRestResponse _restResponse = new RestResponse{ StatusCode = System.Net.HttpStatusCode.Created, ResponseStatus = ResponseStatus.Completed };
		private TestTimer _timer = new TestTimer();
		private BrowserResponse _response;
		//private InMemoryJournaler _journaler;
		private PostgresJournaler _journaler;

		[TestFixtureSetUp]
		public void BeforeAll()
		{
			CleanupDatabase ();
			_restClient = new FakeRestClient (new []{ _restResponse });
			_journaler = new PostgresJournaler (new PostgresCommandFactory (), ConnectionString);
			//_journaler = new InMemoryJournaler ();

			_busFactory = new BusFactory ()
				.WithConnectionString(ConnectionString)
				.WithRestClient (_restClient)
				.WithJournaler (_journaler)
				.WithTimer (_timer);

			_service = new Browser (ServiceConfigurator);

		}

		[TestFixtureTearDown]
		public void AfterAll()
		{
			CleanupDatabase ();
		}

		protected Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> ServiceConfigurator {
			get { return with => {
					with.Module<TModule> ();
					with.Dependency<IBus> (_busFactory.Build ());
					with.ApplicationStartup ((ioc, pipes) => {
						ioc.Resolve<IBus> ().Send (new SystemMessage.Start ());
					});
					with.EnableAutoRegistration ();
				};}
		}

		protected BrowserResponse Response {
			get { return _response; }
		}

		protected string ResponseBody {
			get { return _response.Body.AsString (); }
		}

		protected void GET(string url, Guid reminderId)
		{
			_response = _service.Get (url + reminderId, with => {
				with.Query("reminderId", reminderId.ToString());
			});
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
			_restClient.SetResponseObject (response);
		}

		protected void SetHttpClientResponses(IEnumerable<IRestResponse> responses)
		{
			_restClient.SetResponses (responses);
		}

		protected List<IRestRequest> AllInterceptedHttpRequests {
			get { return _restClient.Requests; }
		}

		protected IRestRequest LastInterceptedHttpRequest{
			get { return _restClient.LastRequest; }
		}

		protected void RestartService()
		{
			_service = new Browser (ServiceConfigurator);
		}

		protected DateTime Now {
			get { return SystemTime.Now (); }
		}

		protected DateTime UtcNow {
			get { return SystemTime.UtcNow (); }
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

