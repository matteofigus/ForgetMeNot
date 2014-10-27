using System;
using System.Collections.Generic;

namespace ReminderService.Clustering
{
	public interface IClusterMembersProvider
	{
		List<Uri> NodesInCluster { get; }
	}
}

