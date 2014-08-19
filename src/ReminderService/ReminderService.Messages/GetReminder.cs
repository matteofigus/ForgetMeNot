using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Messages
{
    public class GetReminder : IRequest<ReminderMessage.Schedule>
    {
        public Guid ReminderId { get; set; }

        public GetReminder(Guid reminderId)
        {
            ReminderId = reminderId;
        }
    }
}
