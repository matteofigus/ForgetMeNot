﻿using System;
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
using OpenTable.Services.Components.RabbitMq;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public abstract class ServiceSpec<TModule> : PostgresTestBase where TModule : NancyModule
	{
		//const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";
		protected Browser _service;
		protected IBusFactory _busFactory;
		private FakeRestClient _restClient;
		private FakeRabbitMqPublisher _rabbitMqPublisher;
		private IRestResponse _restResponse = new RestResponse{ StatusCode = System.Net.HttpStatusCode.Created, ResponseStatus = ResponseStatus.Completed };
		private TestTimer _timer = new TestTimer();
		protected BrowserResponse _response;
		//private InMemoryJournaler _journaler;
		private PostgresJournaler _journaler;

		[TestFixtureSetUp]
		public void BeforeAll()
		{
			CleanupDatabase ();
			_restClient = new FakeRestClient (new []{ _restResponse });
			_rabbitMqPublisher = new FakeRabbitMqPublisher();
			_journaler = new PostgresJournaler (new PostgresCommandFactory (), ConnectionString);
			//_journaler = new InMemoryJournaler ();

			_busFactory = new BusFactory ()
				.WithConnectionString(ConnectionString)
				.WithRestClient (_restClient)
				.WithRabbitMqPublisher(_rabbitMqPublisher)
				.WithJournaler (_journaler)
				.WithTimer (_timer);

			_service = new Browser (ServiceConfigurator);

		}

		[TestFixtureTearDown]
		public void AfterAll()
		{
			CleanupDatabase ();
		}

		protected virtual Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> ServiceConfigurator {
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
			_response = _service.Get (url + reminderId);
		}

		protected void POST(string url, object message)
		{
			_response = _service.Post(url, with => {
				with.JsonBody(message);
			});
		}

		protected void DELETE(string url, Guid reminderId)
		{
			_response = _service.Delete(url + reminderId.ToString());
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

		protected string LastInterceptedRabbitMqConnect
		{
			get { return _rabbitMqPublisher.LastConnectionString; }
		}

		protected byte[] LastInterceptedRabbitMqPublishBody
		{
			get { return _rabbitMqPublisher.LastMessageBody; }
		}   

		protected RoutingParameters LastInterceptedRabbitMqPublishRoutingParameters
		{
			get { return _rabbitMqPublisher.LastRoutingParameters; }
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

		protected void AssertReminderWasSent(Guid reminderId)
		{
			GET ("/reminders/", reminderId);
			Assert.AreEqual (HttpStatusCode.OK, _response.StatusCode);
			Assert.IsTrue (ResponseBody.Contains(reminderId.ToString()));
			Assert.IsTrue (ResponseBody.Contains("Delivered"));
		}
	}
}

