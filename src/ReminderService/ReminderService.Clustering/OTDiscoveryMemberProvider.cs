using System;
using System.Linq;
using System.Collections.Generic;
using OpenTable.Services.Components.DiscoveryClient;

namespace ReminderService.Clustering
{
	public class OTDiscoveryMemberProvider : IClusterMembersProvider
	{
		private readonly object _lockObject = new object ();
		private readonly CSDiscoveryClient _discoveryClient;
		private readonly List<Uri> _clusterMembers;
		private readonly Dictionary<Guid, Uri> _clusterMemberMap = new Dictionary<Guid, Uri> ();

		public OTDiscoveryMemberProvider ()
		{
			_discoveryClient = new CSDiscoveryClient ();
			_discoveryClient.ListenForUpdates (update => {
				if (update.Type == UpdateType.DELETE)
					_clusterMemberMap.Remove(update.AnnouncementId);
				else if (update.Type == UpdateType.UPDATE)
					GetClusterMembers();
			});

			GetClusterMembers();
		}

		private void GetClusterMembers()
		{
			lock (_lockObject) {
				_discoveryClient
					.FindAnnouncements (ServiceLookup.With ().SetServiceType ("ForgetMeNot"))
					.ForEach(m => _clusterMemberMap.Add(m.AnnouncementId, m.ServiceUri));
			}
		}

		public List<Uri> NodesInCluster {
			get {
				lock(_lockObject)
					return _clusterMembers;
			}
		}
	}
}

