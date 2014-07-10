using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ReminderService.Core.ScheduleReminder;

namespace ReminderService.Core.Tests.ScheduleReminder
{
    [TestFixture]
    public class ScheduledReminderTests
    {
        [Test]
        public void Equality_is_based_on_Id()
        {
            var now = DateTime.Now;
            var id = Guid.NewGuid();
            var reminder1 = new ScheduledReminder(id, now, new object(), new Uri("http://desitination/url"));
            var reminder2 = new ScheduledReminder(id, now.AddSeconds(2), new object(), new Uri("http://desitination/url"));

            Assert.True(reminder1.Equals(reminder2));

            var reminder3 = new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url"));

            Assert.False(reminder1.Equals(reminder3));
        }

        [Test]
        public void Comparable_is_based_on_TimeOutAt()
        {
            var now = DateTime.Now;
            var id = Guid.NewGuid();
            var collection = new List<ScheduledReminder>();
            collection.Add(new ScheduledReminder(id, now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(id, now.AddSeconds(10), new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(id, now.AddSeconds(4), new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(id, now.AddSeconds(5), new object(), new Uri("http://desitination/url")));
            collection.Sort();

            CollectionAssert.IsOrdered(collection);
        }
    }
}
