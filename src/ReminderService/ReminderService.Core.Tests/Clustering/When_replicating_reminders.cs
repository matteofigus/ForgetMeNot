using System;
using NUnit.Framework;
using ReminderService.Messages;
using System.Collections.Generic;
using ReminderService.Core.Clustering;
using System.Linq;
using RestSharp;

namespace ReminderService.Core.Tests.Clustering
{
	[TestFixture]
	public class When_replicating_reminders : Given_a_Replicator
	{
		private Guid _reminderId;
		private List<Uri> _nodesInCluster;

		public When_replicating_reminders()
		{
			_nodesInCluster = new List<Uri>{
				new Uri("http://host1:8080/reminders", UriKind.Absolute),
				new Uri("http://host2:8080/reminders", UriKind.Absolute),
			};

			WithReplicatorFactory (() => new Replicator (Bus, RestClient, _nodesInCluster));
			WithResponse (new RestResponse(){
				StatusCode = System.Net.HttpStatusCode.Created,
				ResponseStatus = ResponseStatus.Completed
			});
		}

		[TestFixtureSetUp]
		public void When_receive_a_reminder()
		{
			_reminderId = Guid.NewGuid ();
			var reminder = BuildReminder (_reminderId);

			Assert.AreEqual (0, MessagesReceivedOnTheBus.Count);
			HandleMessage (reminder);
		}

		[Test]
		public void Then_the_replicator_should_have_attempted_to_contact_each_node()
		{
			Assert.AreEqual (2, RestClient.Requests.Count);
			foreach (var node in _nodesInCluster) {
				Assert.IsTrue (RestClient.Requests.Exists(r => r.Resource == node.AbsolutePath));
			}
		}
	}
}

