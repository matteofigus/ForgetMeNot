using System;
using OpenTable.Services.Components.DiscoveryClient;
using System.Collections.Generic;
using System.Linq;

namespace ReminderService.Clustering.OTDiscovery.Tests
{
	public class FakeDiscoveryClient : IDiscoveryClient, IAnnouncementRegistry
	{
		private List<Announcement> _announcements;
		private Action<AnnouncementUpdate> _updatedAnnouncementCallback;

		public FakeDiscoveryClient (IEnumerable<Uri> serviceLocations)
		{
			_announcements = BuildAnnouncements (serviceLocations);
		}
			
		public void UpdateAnnouncements(List<Uri> newServiceUris)
		{
			_announcements = BuildAnnouncements (newServiceUris);
			_updatedAnnouncementCallback (new AnnouncementUpdate (UpdateType.UPDATE, Guid.NewGuid (), 0));
		}

		public List<Announcement> FindAnnouncements (ServiceLookup lookup, params LookupHint[] hints)
		{
			return _announcements;
		}

		public void ListenForUpdates (Action<AnnouncementUpdate> updatedAnnouncementCallback)
		{
			_updatedAnnouncementCallback = updatedAnnouncementCallback;
		}

		public IAnnouncementLease CreateAnnouncement (AnnouncementBuilder announcementBuilder)
		{
			throw new NotImplementedException ();
		}

		public bool Unannounce (Guid announcementId)
		{
			throw new NotImplementedException ();
		}

		public List<Announcement> GetLocalAnnouncements ()
		{
			throw new NotImplementedException ();
		}

		public List<IAnnouncementLease> GetLocalAnnouncementLeases ()
		{
			throw new NotImplementedException ();
		}

		private static List<Announcement> BuildAnnouncements(IEnumerable<Uri> serviceLocations)
		{
			return serviceLocations.Select (uri => 
				new Announcement(Guid.NewGuid(),
					false,
					DateTime.Now.Ticks,
					"ForgetMeNot",
					"feature",
					uri,
					null
				)
			).ToList();
		}
	}
}

