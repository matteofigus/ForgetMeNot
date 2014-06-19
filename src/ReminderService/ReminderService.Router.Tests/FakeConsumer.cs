using System;

namespace ReminderService.Router.Tests
{
    public class FakeConsumer<T> : IConsume<T> where T : IMessage
    {
        private readonly Action<T> _consumerDelegate;

        public FakeConsumer(Action<T> consumerDelegate)
        {
            _consumerDelegate = consumerDelegate;
        }

        public void Handle(T msg)
        {
            _consumerDelegate(msg);
        }
    }
}
