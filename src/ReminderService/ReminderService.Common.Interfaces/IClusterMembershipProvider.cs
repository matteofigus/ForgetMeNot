using System;
using System.Collections.Generic;

namespace ReminderService.Common.Interfaces
{
	public interface IClusterMembershipProvider
	{
		List<Uri> NodesInCluster { get; }
	}
}

