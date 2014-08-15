using System;
using System.Collections.Concurrent;
using ReminderService.Common;
using ReminderService.Router.Consumers;
using ReminderService.Router.Topics;
using log4net;

namespace ReminderService.Router
{
    public class Bus : IBus
    {
		private static readonly ILog Logger = LogManager.GetLogger(typeof(Bus));
        private readonly ITopicFactory<Type> _messageTypeTopics = new MessageTypeTopics();
        private readonly ConcurrentDictionary<string, Multiplexer<IMessage>> _subscribers
            = new ConcurrentDictionary<string, Multiplexer<IMessage>>();

//		public Bus (ITopicFactory<Type> topicFactory)
//		{
//			Ensure.NotNull (topicFactory, "topicFactory");
//			_messageTypeTopics = topicFactory;
//		}

        public void Send(IMessage message)
        {
            var topics = _messageTypeTopics.GetTopicsFor(message.GetType());
            foreach (var topic in topics)
            {
                SendToTopic(topic, message);
            }
        }

        private void SendToTopic(string topic, IMessage message)
        {
            Multiplexer<IMessage> multiplexer;
            if (_subscribers.TryGetValue(topic, out multiplexer))
                multiplexer.Handle(message);
        }

		public void Subscribe<T>(IConsume<T> consumer) where T : class, IMessage
        {
			var wideningConsumer = new WideningConsumer<T, IMessage>(consumer);
			_subscribers.AddOrUpdate (typeof(T).FullName,
				s => new Multiplexer<IMessage> (wideningConsumer),
				(_, multiplexer) => {
					multiplexer.Attach (wideningConsumer);
					return multiplexer;
				});
        }

		public void Subscribe<TRequest, TResponse> (IHandleQueries<TRequest, TResponse> queryhandler) where TRequest : IRequest<TResponse>
		{
			throw new System.NotImplementedException ();
		}

        public void ClearSubscribers()
        {
            _subscribers.Clear();
        }
    }
}
