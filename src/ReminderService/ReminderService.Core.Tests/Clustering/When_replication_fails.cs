using System;
using ReminderService.Core.Clustering;
using System.Collections.Generic;
using NUnit.Framework;
using ReminderService.Messages;
using System.Linq;
using RestSharp;
using System.Net;

namespace ReminderService.Core.Tests.Clustering
{
	public class When_replication_fails : Given_a_Replicator
	{
		private Guid _reminderId;
		private List<Uri> _nodesInCluster;
		
		public When_replication_fails ()
		{
			_nodesInCluster = new List<Uri>{
				new Uri("http://host1:8080/reminders", UriKind.Absolute),
				new Uri("http://host2:8080/reminders", UriKind.Absolute),
			};

			WithFactory (() => new Replicator (Bus, RestClient, _nodesInCluster));
			WithResponse (new RestResponse{StatusCode = HttpStatusCode.BadRequest});

		}

		[TestFixtureSetUp]
		public void When_receive_a_reminder()
		{
			_reminderId = Guid.NewGuid ();
			var reminder = BuildReminder (_reminderId);
			var replicateMe = new ClusterMessage.Replicate<ReminderMessage.Schedule> (reminder);

			HandleMessage (replicateMe);
		}

		[Test]
		public void Then_the_replicator_should_have_attempted_to_contact_each_node()
		{
			Assert.AreEqual (2, RestClient.Requests.Count);
			foreach (var node in _nodesInCluster) {
				Assert.IsTrue (RestClient.Requests.Exists(r => r.Resource == node.AbsolutePath));
			}
		}

		[Test]
		public void Then_the_replicator_should_emit_replication_failed_events()
		{
			Assert.AreEqual (3, MessagesReceivedOnTheBus.Count);
			Assert.IsInstanceOf<ClusterMessage.ReplicationFailed> (MessagesReceivedOnTheBus.First());
			Assert.IsInstanceOf<ClusterMessage.ReplicationFailed> (MessagesReceivedOnTheBus[1]);
		}

		[Test]
		public void Then_the_replicator_should_emit_a_Replicated_event()
		{
			Assert.IsInstanceOf<ClusterMessage.Replicated<ReminderMessage.Schedule>> (MessagesReceivedOnTheBus[2]);
		}
	}
}

