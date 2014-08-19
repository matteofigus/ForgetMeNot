using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ReminderService.Router.Consumers;
using ReminderService.Router.Topics;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router
{
	//playing arround with implementations
	//this bus uses a List of a non-generic wrapper type that delegates messages to their
	//wrapped IConsumer<T> instance - avoids the generics in Bus class
	public class Bus : IBus
    {
        private readonly ITopicFactory<Type> _messageTypeTopics = new MessageTypeTopics();
		private readonly ConcurrentDictionary<string, List<IDispatchMessages>> _subscribers = 
			new ConcurrentDictionary<string, List<IDispatchMessages>>();
		private readonly ConcurrentDictionary<string, IDispatchQueries> _queryHandlers =
			new ConcurrentDictionary<string, IDispatchQueries> ();

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
			List<IDispatchMessages> handlers;
			if (_subscribers.TryGetValue (topic, out handlers)) {
				foreach (var handler in handlers) {
					handler.Dispatch (message);
				}
			}
        }

		public TResponse Send<TResponse>(IRequest<TResponse> query)
		{
			return (TResponse) _queryHandlers[query.GetType().FullName].Dispatch(query);
		}

		public void Subscribe<T>(IConsume<T> consumer) where T : class, IMessage
        {
			IDispatchMessages handler = new MessageDispatcher<T> (consumer);
			_subscribers.AddOrUpdate (
				typeof(T).FullName,
				s => {
					var list = new List<IDispatchMessages> ();
					list.Add(handler);
					return list;
				},
				(_, list) => {
					list.Add (handler);
					return list;
				});
        }

		public void Subscribe<TRequest, TResponse> (IHandleQueries<TRequest, TResponse> queryhandler) where TRequest : IRequest<TResponse>
		{
            if (_queryHandlers.ContainsKey(typeof(TRequest).FullName))
                throw new InvalidOperationException(
                    string.Format("Cannot subscribe because a query handler is already registered for request of type [{0}]", typeof(TRequest).FullName));

			_queryHandlers.AddOrUpdate (
				typeof(TRequest).FullName,
				new QueryDispatcher<TRequest, TResponse>(queryhandler),
				(_, dispatcher) => 
					new QueryDispatcher<TRequest, TResponse>(queryhandler)
			);
		}

        public void ClearSubscribers()
        {
            _subscribers.Clear();
        }
    }
}
