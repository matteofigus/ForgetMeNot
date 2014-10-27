using System;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;
using System.Collections.Generic;
using RestSharp;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using System.Reactive.Linq;
using log4net;
using ReminderService.Clustering;
using ReminderService.Common.Interfaces;

namespace ReminderService.Core.Clustering
{
	public class Replicator : 
		IConsume<ReminderMessage.Schedule>
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(Replicator));
		private readonly object _lockObject = new object ();
		private const int TimeoutMs = 1000;
		private readonly ISendMessages _bus;
		private readonly IRestClient _restClient;
		private readonly IClusterMembershipProvider _clusterMembershipProvider;

		public Replicator (ISendMessages bus, IRestClient restClient, IClusterMembershipProvider membershipProvider)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (restClient, "restClient");
			Ensure.NotNull (membershipProvider, "membershipProvider");

			_bus = bus;
			_restClient = restClient;
			_clusterMembershipProvider = membershipProvider;
		}

		public void Handle(ReminderMessage.Schedule msg)
		{
			// iterate the Uri of each other node in the cluster, 
			// call SendToNode that returns a Task, 
			// turn each Task to an Observable and merge all the observables together =>
			// we have created a stream of task results (IRestResponse) and results will be pushed as each Task completes
			// subscribe to the completed responses and check the state of each response to make sure we replicated the message.
			lock (_lockObject) {
				_clusterMembershipProvider
				.NodesInCluster
				.Select (uri => SendToNode (msg, uri))
				.ToObservable ()
				.Merge ()
				.Subscribe (
					response => {
						if (response.ResponseStatus == ResponseStatus.Completed) {
							if (response.StatusCode != HttpStatusCode.Created)
								OnHttpError (msg, response);
						} else
							OnTransportError (msg, response);
					},
					exception => OnException (msg, exception),
					() => OnReplicated (msg)
				);
			}
		}

		private Task<IRestResponse> SendToNode(ReminderMessage.Schedule msg, Uri node)
		{
			var request = new RestRequest (node, Method.POST)
			{ RequestFormat = DataFormat.Json }
				.AddParameter ("application/json", msg, ParameterType.RequestBody);

			return _restClient.ExecutePostTaskAsync (request);
		}

		private void OnReplicated(ReminderMessage.Schedule msg)
		{
			Logger.InfoFormat ("Reminder [{0}] replicated.", msg.ReminderId);
		}

		private void OnTransportError(ReminderMessage.Schedule msg, IRestResponse response)
		{
			var message = string.Format ("There was a transport level exception while attempting to replicate a message to the node [{0}] in the cluster.", response.ErrorMessage, response.ErrorException);
			Logger.Error (message);
			_bus.Send (new ClusterMessage.ReplicationFailed(message));
		}

		private void OnHttpError(ReminderMessage.Schedule msg, IRestResponse response)
		{
			var message = string.Format ("Received an unexpected HTTPStatusCode [{0} - {1}] while attempting to replicate reminder [{2}] to the other nodes in the cluster.", response.StatusCode, response.StatusDescription, msg.ReminderId);
			Logger.Error (message);
			_bus.Send (new ClusterMessage.ReplicationFailed(message));
		}

		private void OnException(ReminderMessage.Schedule msg, Exception ex)
		{
			var message = string.Format("An exception was thrown while attempting to replicate the reminder [{0}] to other nodes in the cluster.", msg.ReminderId);
			Logger.Error (message, ex);
			_bus.Send (new ClusterMessage.ReplicationFailed(ex, message));
		}
	}
}

