using System;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;
using System.Collections.Generic;

namespace ReminderService.Messages
{
	public static class ClusterMessage
	{
		public class ReplicationFailed : IMessage
		{
			public Exception Exception {
				get;
				private set;
			}

			public string Message {
				get;
				private set;
			}

			public ReplicationFailed (Exception ex, string message)
			{
				Ensure.NotNull(ex, "ex");
				Ensure.NotNullOrEmpty(message, "message");

				Exception = ex;
				Message = message;
			}

			public ReplicationFailed (string message)
			{
				Ensure.NotNullOrEmpty(message, "message");
				Message = message;
			}
		}

		public class MembershipUpdate : IMessage
		{
			public List<Uri> NewMembershipList { get; set; }

			public MembershipUpdate (List<Uri> members)
			{
				Ensure.NotNull(members, "members");
				NewMembershipList = members;
			}
		}
	}
}

