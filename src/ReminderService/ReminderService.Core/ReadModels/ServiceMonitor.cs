using System;
using System.Collections.Generic;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Common;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core.ReadModels
{
	public class ServiceMonitor : 
		IConsume<ReminderMessage.Undeliverable>,
		IConsume<ReminderMessage.Delivered>,
		IHandleQueries<QueryResponse.GetServiceMonitorState, QueryResponse.ServiceMonitorState>
	{
		private readonly Object _lockObject = new Object();
		private readonly QueryResponse.ServiceMonitorState _state = new QueryResponse.ServiceMonitorState();

		public ServiceMonitor ()
		{
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
				return _state;
			}
		}

		const string Undelivered_Key = "UndeliveredCount";
	}
}

