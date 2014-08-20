using System;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Common;

namespace ReminderService.Messages
{
	public static class Queries
	{
	    public class GetReminder : IRequest<Maybe<Responses.CurrentReminderState>>
	    {
	        public Guid ReminderId { get; set; }

	        public GetReminder(Guid reminderId)
	        {
	            ReminderId = reminderId;
	        }
	    }
	}
}
