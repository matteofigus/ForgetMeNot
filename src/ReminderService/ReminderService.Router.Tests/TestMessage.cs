using System;

namespace ReminderService.Router.Tests
{
    public class AnEvent : IMessage
    {
        
    }

    public class TestMessage : IMessage
    {
        private readonly string _id;
        private readonly DateTime _created;

        public TestMessage()
        {
            //empty default constructor
        }

        public TestMessage(string id, DateTime created)
        {
            _id = id;
            _created = created;
        }

        public string Id
        {
            get { return _id; }
        }

        public DateTime Created
        {
            get { return _created; }
        }
    }
}
