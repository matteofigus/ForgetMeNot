using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using ReminderService.Core.ScheduleReminder;

namespace ReminderService.Core.Tests.ScheduleReminder
{
    [TestFixture]
    public class ScheduledReminderCollectionTests
    {
        [Test]
        public void Add()
        {
            var now = DateTime.Now;
            var collection = new ScheduledReminderGroup(now);
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));

            Assert.AreEqual(4, collection.Count);
        }

        [Test]
        public void Add_fails_for_differing_timeouts()
        {
            var now = DateTime.Now;
            var collection = new ScheduledReminderGroup(now);
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));

            Assert.Throws<ArgumentOutOfRangeException>(
                () =>
                collection.Add(new ScheduledReminder(Guid.NewGuid(), now.AddSeconds(1), new object(), new Uri("http://desitination/url"))));
        }

        [Test]
        public void Constructor_from_a_collection()
        {
            var now = DateTime.Now;
            var collection = new List<ScheduledReminder>();
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));

            var reminderCollection = new ScheduledReminderGroup(now, collection);

            Assert.AreEqual(4, reminderCollection.Count);
        }

        [Test]
        public void Constructor_throws_if_timeouts_are_not_all_the_same()
        {
            var now = DateTime.Now;
            var collection = new List<ScheduledReminder>();
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now.AddSeconds(1), new object(), new Uri("http://desitination/url")));
            collection.Add(new ScheduledReminder(Guid.NewGuid(), now, new object(), new Uri("http://desitination/url")));

            ScheduledReminderGroup reminderGroup = null;
            Assert.Throws<ArgumentOutOfRangeException>(
                () => reminderGroup = new ScheduledReminderGroup(now, collection));
        }
    }
}
