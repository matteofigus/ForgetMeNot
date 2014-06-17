using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reactive;

namespace ReminderService.DataStructures
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
