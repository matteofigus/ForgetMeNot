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
using ReminderService.API.HTTP.Modules;
using ReminderService.API.HTTP.Monitoring;
using System.Diagnostics;
using OpenTable.Services.Components.Monitoring.Monitors.HitTracker;

namespace ReminderService.API.HTTP.Tests.ServiceMonitoring
{
	public class Given_the_service_is_configured_with_monitoring : ServiceSpec<ReminderApiModule>
	{
		const string RequestTimerKey = "RequestTimer";
		const string RequestStartTimeKey = "RequestStartTime";

		protected override Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> ServiceConfigurator {
			get {
				return with => {
					var hitTracker = new HitTracker (HitTrackerSettings.Instance);
					with.Modules (typeof(ReminderApiModule), typeof(ServiceMonitoringModule));
					with.Dependency<IBus> (_busFactory.Build ());
					with.Dependency<HitTracker> (hitTracker);
					with.RequestStartup((container, pipelines, context) => {
						//response time
						pipelines.BeforeRequest.AddItemToStartOfPipeline (ctx => {
							ctx.Items[RequestTimerKey] = Stopwatch.StartNew();
							ctx.Items[RequestStartTimeKey] = SystemTime.UtcNow();
							return null;
						});

						pipelines.AfterRequest.AddItemToEndOfPipeline ((ctx) => {
							Maybe<DateTime> maybeStarted = MaybeGetItem<DateTime>(RequestStartTimeKey, ctx);
							Maybe<Stopwatch> maybeTimer = MaybeGetItem<Stopwatch>(RequestTimerKey, ctx);

							if(maybeTimer.HasValue) {
								var stopwatch = maybeTimer.Value;
								stopwatch.Stop();
								var elapsedMs = stopwatch.ElapsedMilliseconds;
								ctx.Items.Remove(RequestTimerKey);

								container
									.Resolve<HitTracker>()
									.AppendHit(ctx.Request.Url.ToString(), new Hit {
										StartTime = maybeStarted.HasValue ? maybeStarted.Value : DateTime.MinValue,
										IsError = false,
										TimeTaken = stopwatch.Elapsed
									});
							}
						});
					});
					with.ApplicationStartup ((ioc, pipes) => {
						ioc.Resolve<IBus> ().Send (new SystemMessage.Start ());
					});
					//with.EnableAutoRegistration ();
				};
			}
		}

		protected BrowserResponse GET_ServiceStatus()
		{
			return _service.Get ("/service-status/");
		}

		private static Maybe<T> MaybeGetItem<T>(string key, NancyContext context)
		{
			object requestStart;
			if (context.Items.TryGetValue (key, out requestStart))
				return new Maybe<T> ((T)requestStart);

			return Maybe<T>.Empty;
		}
	}

	[TestFixture ()]
	public abstract class ServiceMonitorSpec_old : PostgresTestBase
	{
		//const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";
		protected Browser _service;
		protected IBusFactory _busFactory;
		private FakeRestClient _restClient;
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

		protected virtual Action<ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator> ServiceConfigurator {
			get { return with => {
					var mediator = new MonitoringMediator();
					with.Modules(typeof(ReminderApiModule), typeof(ServiceMonitoringModule));
					//with.Module<ReminderApiModule>();
					with.Dependency<IBus> (_busFactory.Build ());
					with.Dependency<IMediateEvents> (mediator);
					with.Dependency<IPushEvents> (mediator);
					with.Dependency<HttpApiMonitor>(new HttpApiMonitor(mediator, 10, 1));
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

		protected BrowserResponse GET_ServiceStatus()
		{
			return _service.Get ("/service-status/");
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

