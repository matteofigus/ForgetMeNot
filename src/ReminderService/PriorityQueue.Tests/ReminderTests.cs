using System;
using NUnit.Framework;

namespace ReminderService.DataStructures.Tests
{
    [TestFixture]
    public class ReminderTests
    {

        public class ReminderObserver : IObserver<ScheduledReminder>
        {
            public void OnNext(ScheduledReminder value)
            {
                throw new NotImplementedException();
            }

            public void OnError(Exception error)
            {
                throw new NotImplementedException();
            }

            public void OnCompleted()
            {
                throw new NotImplementedException();
            }
        }
    }
}
