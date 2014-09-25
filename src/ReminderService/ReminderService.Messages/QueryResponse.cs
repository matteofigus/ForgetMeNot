using System;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public static class QueryResponse
	{
	    public class GetReminderState : IRequest<Maybe<QueryResponse.CurrentReminderState>>
	    {
	        public Guid ReminderId { get; set; }

	        public GetReminderState(Guid reminderId)
	        {
	            ReminderId = reminderId;
	        }
	    }

		//todo: make this a static class based enum?
		public enum ReminderStatusEnum
		{
			Scheduled,
			Delivered,
			Canceled,
			Redelivering,
			Undeliverable
		}

		public class CurrentReminderState : IMessage
		{
			public ReminderStatusEnum Status { get; set; }
			public ReminderMessage.Schedule Reminder { get; set; }
			public int RedeliveryAttempts { get; set; }

			public CurrentReminderState ()
			{
				//empty
			}

			public CurrentReminderState (ReminderMessage.Schedule reminder, ReminderStatusEnum status, int redeliveryAttempts = 0)
			{
				Reminder = reminder;
				Status = status;
				RedeliveryAttempts = redeliveryAttempts;
			}
		}

		public class GetServiceMonitorState : IRequest<QueryResponse.ServiceMonitorState>
		{
			//empty
		}

		public class ServiceMonitorState : IMessage
		{
			public DateTime ServiceStartedAt { get; set; }
			public int QueueSize { get; set; }
			public int UndeliveredCount { get; set; }
			public int UndeliverableCount { get; set; }
			public int DeliveredReminderCount { get; set; }

			public ServiceMonitorState ()
			{
				//empty
			}
		}
	}
}
