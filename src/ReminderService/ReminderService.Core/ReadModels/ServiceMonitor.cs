using System;
using ReminderService.Messages;
using ReminderService.Router;

namespace ReminderService.Core.ReadModels
{
	public class ServiceMonitor : 
		IConsume<ReminderMessage.Undelivered>,
		IConsume<ReminderMessage.Undeliverable>,
		IConsume<ReminderMessage.Delivered>,
		IHandleQueries<QueryResponse.GetServiceMonitorState, QueryResponse.ServiceMonitorState>
	{
		private readonly Object _lockObject = new Object();
		private readonly QueryResponse.ServiceMonitorState _state = new QueryResponse.ServiceMonitorState();

		public ServiceMonitor ()
		{
		}

		public void Handle (ReminderMessage.Undelivered msg)
		{
			lock (_lockObject) {
				_state.UndeliveredCount++;
			}
		}

		public void Handle (ReminderMessage.Undeliverable msg)
		{
			lock (_lockObject) {
				_state.UndeliverableCount++;
			}
		}

		public void Handle (ReminderMessage.Delivered msg)
		{
			lock (_lockObject) {
				_state.DeliveredReminderCount++;
			}
		}

		public QueryResponse.ServiceMonitorState Handle (QueryResponse.GetServiceMonitorState request)
		{
			lock (_lockObject) {
				return _state; //hmmm, do we need to clone this instance before returning? is serialization of this going to interfere with anything?
			}
		}
	}
}

