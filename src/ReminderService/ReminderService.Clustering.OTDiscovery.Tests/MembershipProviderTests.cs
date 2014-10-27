using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ReminderService.Clustering.OTDiscovery.Tests
{
	[TestFixture]
	public class MembershipProviderTests
	{
		[Test]
		public void When_membership_changes ()
		{
			var uris = new List<Uri> ();
			var fakeClient = new FakeDiscoveryClient (uris);
			var provider = new OTDiscoveryMemberProvider (fakeClient, fakeClient);

			CollectionAssert.AreEquivalent (uris, provider.NodesInCluster);

			var updatedUris = new List<Uri> ();
			fakeClient.UpdateAnnouncements (updatedUris);

			CollectionAssert.AreEquivalent (updatedUris, provider.NodesInCluster);
		}
	}
}

