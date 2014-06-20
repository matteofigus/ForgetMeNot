using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReminderService.Router;

namespace ReminderService.Router.Tests
{
    [TestFixture]
    public class TestingPolymorphicRouting
    {
        private readonly List<IMessage> _routedMessages = new List<IMessage>();
        private readonly Bus _bus = new Bus();
        private Action<IMessage> _recordRoutedMessages;
		private TestMessages.TestMessage _publishedMessage;
        
        [TestFixtureSetUp]
		public void Given_a_router()
        {
             _recordRoutedMessages = msg => _routedMessages.Add(msg);

            // this consumer should not receive the TestMessage that is published; 
            // it is listening for a AnEvent message that is not derived from TestMessage
			_bus.Subscribe(new FakeConsumer<TestMessages.NotDerivedTestMessage>(_recordRoutedMessages));
            _bus.Subscribe(new FakeConsumer<IMessage>(_recordRoutedMessages));
			_bus.Subscribe(new FakeConsumer<TestMessages.TestMessage>(_recordRoutedMessages));
			_bus.Subscribe(new FakeConsumer<TestMessages.ADerivedTestMessage>(_recordRoutedMessages)); //this comsumer should not receive the message; AnotherTestMessage is too specialized
        }

        [SetUp]
        public void when_a_TestMessage_is_published()
        {
            _routedMessages.Clear();
			_publishedMessage = new TestMessages.TestMessage();
            _bus.Publish(_publishedMessage);
        }

        [Test]
        public void then_only_relevant_subscribers_receive_the_message()
        {
            Assert.AreEqual(2, _routedMessages.Count);
            CollectionAssert.Contains(_routedMessages, _publishedMessage);
        }
    }
}
