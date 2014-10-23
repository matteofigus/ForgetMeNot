using System;
using NUnit.Framework;
using System.Collections.Generic;
using RestSharp;
using System.Net;
using ReminderService.Messages;
using ReminderService.Core.Clustering;
using System.Linq;

namespace ReminderService.Core.Tests.Clustering
{
	[TestFixture]
	public class When_replication_throws_an_exception : Given_a_Replicator
	{
		private Guid _reminderId;
		private List<Uri> _nodesInCluster;

		public When_replication_throws_an_exception ()
		{
			_nodesInCluster = new List<Uri>{
				new Uri("http://host1:8080/reminders", UriKind.Absolute),
				new Uri("http://host2:8080/reminders", UriKind.Absolute),
			};

			WithReplicatorFactory (() => new Replicator (Bus, RestClient, _nodesInCluster));
			WithRequestHandler ((IRestRequest, callback) => {
				throw new Exception();
			});

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
			Assert.AreEqual (1, RestClient.Requests.Count);
			foreach (var node in _nodesInCluster) {
				Assert.IsTrue (RestClient.Requests.Exists(r => r.Resource == node.AbsolutePath));
			}
		}

		[Test]
		public void Then_the_replicator_should_emit_replication_failed_events()
		{
			Assert.AreEqual (1, MessagesReceivedOnTheBus.Count);
			Assert.IsInstanceOf<ClusterMessage.ReplicationFailed> (MessagesReceivedOnTheBus.First());
		}

		[Test]
		public void Then_the_replicator_should_not_emit_a_Replicated_event()
		{
			Assert.AreEqual (1, MessagesReceivedOnTheBus.Count);
			Assert.IsNotInstanceOf<ClusterMessage.Replicated<ReminderMessage.Schedule>> (MessagesReceivedOnTheBus.First());
		}
	}
}

