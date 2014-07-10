using System;
using System.Collections;
using System.Collections.Generic;

namespace ReminderService.Core.ScheduleReminder
{
    public class ScheduledReminderGroup : IEnumerable<ScheduledReminder>
    {
        private readonly DateTime _timeoutAt;
        private readonly List<ScheduledReminder> _reminders;

        public ScheduledReminderGroup(DateTime timeoutAt) :
            this(timeoutAt, new List<ScheduledReminder>())
        {
        }

        public ScheduledReminderGroup(DateTime timeoutAt, IEnumerable<ScheduledReminder> reminders)
        {
            _timeoutAt = timeoutAt;
            _reminders = new List<ScheduledReminder>();
            foreach (var r in reminders)
            {
                Add(r);
            }
        }

        public ScheduledReminderGroup(ScheduledReminderGroup other)
            : this(other.TimeOutAt, other)
        {
            //copy constuctor
        }

        public int Count
        {
            get { return _reminders.Count; }
        }

        public DateTime TimeOutAt
        {
            get { return _timeoutAt; }
        }
 
        public void Add(ScheduledReminder reminder)
        {
            if(reminder.TimeOutAt.CompareTo(_timeoutAt) != 0)
                throw new ArgumentOutOfRangeException("reminder", "Can not add a reminder with a different TimeOut value.");

            _reminders.Add(reminder);
        }

        IEnumerator<ScheduledReminder> IEnumerable<ScheduledReminder>.GetEnumerator()
        {
            return (IEnumerator<ScheduledReminder>) GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) _reminders).GetEnumerator();
        }
    }
}
