using System;
using System.Linq;
using System.Collections.Generic;
using OpenTable.Services.Components.DiscoveryClient;
using ReminderService.Common.Interfaces;
using ReminderService.Common;

namespace ReminderService.Clustering.OTDiscovery
{
	public class OTDiscoveryMemberProvider : IClusterMembershipProvider
	{
		private readonly object _lockObject = new object ();
		private readonly IDiscoveryClient _discoveryClient;
		private readonly IAnnouncementRegistry _announcementRegistry;
		private readonly Dictionary<Guid, Uri> _clusterMemberMap = new Dictionary<Guid, Uri> ();

		public OTDiscoveryMemberProvider (IDiscoveryClient discoveryClient, IAnnouncementRegistry announcementRegistry)
		{
			Ensure.NotNull (discoveryClient, "discoveryClient");
			Ensure.NotNull (announcementRegistry, "announcementRegistry");

			_discoveryClient = discoveryClient;
			_announcementRegistry = announcementRegistry;

			_announcementRegistry.ListenForUpdates (update => {
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
					return _clusterMemberMap.Values.ToList();
			}
		}
	}
}

