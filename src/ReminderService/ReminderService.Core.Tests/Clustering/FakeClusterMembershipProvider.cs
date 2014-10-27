using System;
using ReminderService.Common.Interfaces;
using System.Collections.Generic;

namespace ReminderService.Core.Tests.Clustering
{
	public class FakeClusterMembershipProvider : IClusterMembershipProvider
	{
		private readonly List<Uri> _uris;

		public FakeClusterMembershipProvider (List<Uri> uris)
		{
			_uris = uris;
		}

		public List<Uri> NodesInCluster {
			get {
				return _uris;
			}
		}
	}
}

