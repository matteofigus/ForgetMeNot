using System;
using ReminderService.Router;
using ReminderService.Test.Common;
using ReminderService.Core.Clustering;
using System.Collections.Generic;
using ReminderService.Router.MessageInterfaces;
using NUnit.Framework;
using RestSharp;
using System.Net;
using ReminderService.Messages;

namespace ReminderService.Core.Tests.Clustering
{
	public class Given_a_Replicator
	{
		private ISendMessages _bus;
		private FakeRestClient _restClient;
		private Replicator _replicator;
		private List<IMessage> _messagesReceivedOnBus = new List<IMessage>();
		private List<IRestResponse> _restResponses;
		private bool _constructWithHandler = false;

		public List<IMessage> MessagesReceivedOnTheBus {
			get { return _messagesReceivedOnBus; }
		}

		public List<IRestResponse> RestResponses {
			get;
			set;
		}

		public ISendMessages Bus {
			get;
			set;
		}

		public FakeRestClient RestClient {
			get;
			set;
		}

		public Func<Replicator> ReplicatorFactory {
			get;
			set; 
		}

		public Action<IRestRequest, Action<IRestResponse, RestRequestAsyncHandle>> RequestHandler {
			get;
			set;
		}

		[TestFixtureSetUp]
		public void SetupFixture ()
		{
			Bus = new FakeBus (msg => _messagesReceivedOnBus.Add(msg));
			RestClient = _constructWithHandler ? new FakeRestClient(RequestHandler) : new FakeRestClient (RestResponses);
			_replicator = ReplicatorFactory ();
		}

		protected void HandleMessage(ReminderMessage.Schedule replicateMe)
		{
			_replicator.Handle (replicateMe);
		}

		protected static ReminderMessage.Schedule BuildReminder(Guid reminderId)
		{
			return new ReminderMessage.Schedule (reminderId,
				DateTime.Now,
				"delivery",
				"application/json",
				ReminderMessage.ContentEncodingEnum.utf8,
				ReminderMessage.TransportEnum.http,
				new byte[0],
				0
			);
		}

		public void WithReplicatorFactory(Func<Replicator> replicatorFactory)
		{
			ReplicatorFactory = replicatorFactory;
		}

		public void WithRequestHandler(Action<IRestRequest, Action<IRestResponse, RestRequestAsyncHandle>> requestHandler)
		{
			RequestHandler = requestHandler;
			_constructWithHandler = true;
		}

		public void WithResponses(IEnumerable<IRestResponse> fakeRestResponses)
		{
			RestResponses = new List<IRestResponse>(fakeRestResponses);
		}

		public void WithResponse(IRestResponse fakeRestResponse)
		{
			if (RestResponses == null)
				RestResponses = new List<IRestResponse> ();

			RestResponses.Add (fakeRestResponse);
		}
	}
}

