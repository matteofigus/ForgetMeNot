using NUnit.Framework;
using System;
using Nancy.Testing;
using Nancy;
using RestSharp;
using ReminderService.Test.Common;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public abstract class ServiceSpec<TModule> where TModule : NancyModule
	{
		private Browser _service;
		private FakeRestClient _restClient;
		private IRestResponse _restResponse = new RestResponse{ StatusCode = System.Net.HttpStatusCode.Created, ResponseStatus = ResponseStatus.Completed };
		private TestTimer _timer = new TestTimer();
		private BrowserResponse _response;

		[TestFixtureSetUp]
		public void BeforeAll()
		{
			_restClient = new FakeRestClient (new []{ _restResponse });
			var busFactory = new BusFactory ()
				.WithRestClient (_restClient)
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

		protected void SetHttpClientResponse(IRestResponse response)
		{
			_restResponse = response;
		}

		protected IRestRequest DeliveryRequest{
			get { return _restClient.LastRequest; }
		}

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

