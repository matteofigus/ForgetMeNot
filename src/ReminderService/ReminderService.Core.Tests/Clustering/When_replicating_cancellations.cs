using System;
using NUnit.Framework;
using System.Collections.Generic;
using RestSharp;
using System.Net;
using ReminderService.Messages;
using System.Linq;

namespace ReminderService.Core.Tests.Clustering
{
	[TestFixture]
	public class When_replicating_cancellations : Given_a_Replicator
	{
		private Guid _reminderId;

		public When_replicating_cancellations()
		{
			var nodesInCluster = new List<Uri>{
				new Uri("http://host1:8080/reminders", UriKind.Absolute),
				new Uri("http://host2:8080/reminders", UriKind.Absolute),
			};

			WithClusterMembers (nodesInCluster);
			WithResponse (new RestResponse(){
				StatusCode = HttpStatusCode.Created,
				ResponseStatus = ResponseStatus.Completed
			});
		}

		[TestFixtureSetUp]
		public void When_receive_a_cancellation()
		{
			_reminderId = Guid.NewGuid ();
			var cancellation = new ReminderMessage.Cancel (_reminderId);

			Assert.AreEqual (0, MessagesReceivedOnTheBus.Count);
			HandleMessage (cancellation);
		}

		[Test]
		public void Then_the_replicator_should_have_attempted_to_contact_each_node()
		{
			Assert.AreEqual (2, RestClient.Requests.Count);
			foreach (var node in ClusterMembers) {
				Assert.IsTrue (RestClient.Requests.Exists(r => r.Resource == node.AbsolutePath));
			}
		}

		[Test]
		public void Then_the_request_contains_the_url_querystring()
		{
			RestClient.Requests.Exists (req => 
				req.Parameters.Any(parameter => 
					parameter.Type == ParameterType.QueryString && parameter.Name == "replicated" && (string)parameter.Value == "true"));
		}
	}
}

