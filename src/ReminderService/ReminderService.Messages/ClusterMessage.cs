using System;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;
using System.Collections.Generic;

namespace ReminderService.Messages
{
	public static class ClusterMessage
	{
		public class Replicate<T> : IMessage where T : class, IMessage
		{
			public T Reminder { get; private set; }

			public Replicate (T reminder)
			{
				Ensure.NotNull(reminder, "reminder");
				Reminder = reminder;
			}
		}

		public class Replicated<T> : IMessage where T : class, IMessage
		{
			public T Reminder { get; private set; }

			public Replicated (T reminder)
			{
				Ensure.NotNull(reminder, "reminder");
				Reminder = reminder;
			}
		}

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
	}
}

