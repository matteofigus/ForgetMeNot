using System;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public static class RequestResponse
	{
	    public class GetReminderState : IRequest<Maybe<RequestResponse.CurrentReminderState>>
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
			Undeliverable
		}

		public class CurrentReminderState : IMessage
		{
			public ReminderMessage.Schedule Reminder { get; set; }
			public ReminderStatusEnum Status { get; set; }
		}
	}
}
