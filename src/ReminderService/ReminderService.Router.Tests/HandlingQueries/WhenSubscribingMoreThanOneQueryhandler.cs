using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ReminderService.Router.Tests.Helpers;

namespace ReminderService.Router.Tests.HandlingQueries
{
    [TestFixture]
    public class WhenSubscribingMoreThanOneQueryhandler : Given_a_bus_instance
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Initialize_bus()
        {
            WithQueryHandler(new FakeQueryHandler<TestRequest, TestResponse>(r => new TestResponse()));
            WithQueryHandler(new FakeQueryHandler<TestRequest, TestResponse>(r => new TestResponse()));
        }
    }
}
