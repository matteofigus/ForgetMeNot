using System;
using System.Collections.Generic;

namespace ReminderService.Core.ScheduleReminder
{
    public class ScheduledReminder : IComparable<ScheduledReminder>, IEquatable<ScheduledReminder>
    {
        public ScheduledReminder(Guid reminderId, DateTime timeOutAt, object payload, Uri destination)
        {
            ReminderId = reminderId;
            TimeOutAt = timeOutAt;
            Payload = payload;
            Destination = destination;
        }
        
        public Guid ReminderId { get; private set; }
        public DateTime TimeOutAt { get; private set; }
        public object Payload { get; private set; }
        public Uri Destination { get; private set; }

        /// <summary>
        /// Comparison for sorting is based on the TimeOutAt
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ScheduledReminder other)
        {
            return TimeOutAt.CompareTo(other.TimeOutAt);
        }

        /// <summary>
        /// Instance equality is based on the Guid
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ScheduledReminder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ReminderId.Equals(other.ReminderId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScheduledReminder) obj);
        }

        public override int GetHashCode()
        {
            return ReminderId.GetHashCode();
        }

        private sealed class TimeOutAtEqualityComparer : IEqualityComparer<ScheduledReminder>
        {
            public bool Equals(ScheduledReminder x, ScheduledReminder y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.TimeOutAt.Equals(y.TimeOutAt);
            }

            public int GetHashCode(ScheduledReminder obj)
            {
                return obj.TimeOutAt.GetHashCode();
            }
        }

        private static readonly IEqualityComparer<ScheduledReminder> TimeOutAtComparerInstance = new TimeOutAtEqualityComparer();

        public static IEqualityComparer<ScheduledReminder> TimeOutAtComparer
        {
            get { return TimeOutAtComparerInstance; }
        }
    }
}
