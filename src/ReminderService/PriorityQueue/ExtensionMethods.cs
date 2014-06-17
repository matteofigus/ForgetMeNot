using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;

namespace ReminderService.DataStructures
{
    public static class ExtensionMethods
    {
        public static ScheduledReminderGroup MergeWith(this ScheduledReminderGroup source, ScheduledReminderGroup other)
        {
            if(source.TimeOutAt.CompareTo(other) != 0)
                throw new InvalidOperationException("Cannot merge collections that do not have the same TimeOutAt values.");

            var merged = new ScheduledReminderGroup(source);
            foreach (ScheduledReminder scheduledReminder in other)
            {
                merged.Add(scheduledReminder);
            }
            return merged;
        }

		public static IEnumerable<ScheduledReminder> GetRemindersAtTime(this MinPriorityQueue<ScheduledReminder> pq, DateTime time)
		{
			var reminders = new List<ScheduledReminder>();
			var reminder = pq.RemoveMin ();
			reminders.Add (reminder);
			while(!pq.IsEmpty && reminder.TimeOutAt.CompareTo(time) == 0)
			{
				reminders.Add (reminder);
				reminder = pq.RemoveMin();
			}
				
			return reminders;
		}

		public static IObservable<ScheduledReminder> AsObservable(this MinPriorityQueue<ScheduledReminder> source)
        {
            var time = Observable.Interval(TimeSpan.FromMilliseconds(100));
            var reminders = Observable.Generate(
				source.RemoveMin(),
                //get the first element off the queue - what if it is null? (we need a functor / monad!)
                i => !source.IsEmpty, //probably just want to return true so that this is infinite
				i => source.RemoveMin(),
                i => i);

			//var remindersByTime = time.GroupJoin(reminders, x => x, x => x.TimeOutAt, (t, remindersDue) => remindersDue);

			return reminders;
        }
    }
}
