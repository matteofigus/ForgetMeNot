using System;
using ReminderService.Router;
using ReminderService.Messages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ReminderService.Common;

namespace ReminderService.Core.ReadModels
{
	public class CurrentStateOfReminders : 
		IConsume<Envelopes.Journaled<ReminderMessage.Schedule>>,
		IConsume<Envelopes.Journaled<ReminderMessage.Cancel>>,
		IConsume<ReminderMessage.Delivered>,
		IConsume<ReminderMessage.Undelivered>,
		IConsume<ReminderMessage.Undeliverable>,
		IHandleQueries<QueryResponse.GetReminderState, Maybe<QueryResponse.CurrentReminderState>>
	{
		private readonly object lockObject = new object();
		private readonly Dictionary<Guid, QueryResponse.CurrentReminderState> _states = 
			new Dictionary<Guid, QueryResponse.CurrentReminderState>();

		public CurrentStateOfReminders ()
		{
		}

		public void Handle (Envelopes.Journaled<ReminderMessage.Schedule> envelope)
		{
			lock (lockObject) {
				if (!_states.ContainsKey (envelope.Message.ReminderId)) {
					_states.Add (envelope.Message.ReminderId, 
						new QueryResponse.CurrentReminderState (
							envelope.Message,
							QueryResponse.ReminderStatusEnum.Scheduled
						)
					);
				}
			}
		}

		public void Handle (Envelopes.Journaled<ReminderMessage.Cancel> envelope)
		{
			lock (lockObject) {
				if (_states.ContainsKey (envelope.Message.ReminderId))
					_states [envelope.Message.ReminderId].Status = QueryResponse.ReminderStatusEnum.Canceled;
			}
		}

		public void Handle (ReminderMessage.Delivered sent)
		{
			lock (lockObject) {
				if (_states.ContainsKey (sent.ReminderId))
					_states [sent.ReminderId].Status = QueryResponse.ReminderStatusEnum.Delivered;
			}
		}

		public void Handle (ReminderMessage.Undelivered undelivered)
		{
			lock (lockObject) {
				if (_states.ContainsKey (undelivered.ReminderId))
					_states [undelivered.ReminderId].RedeliveryAttempts++;
			}
		}

		public void Handle (ReminderMessage.Undeliverable undeliverable)
		{
			lock (lockObject) {
				if (_states.ContainsKey (undeliverable.ReminderId))
					_states [undeliverable.ReminderId].Status = QueryResponse.ReminderStatusEnum.Undeliverable;
			}
		}

		public Maybe<QueryResponse.CurrentReminderState> Handle (QueryResponse.GetReminderState request)
		{
			lock (lockObject) {
				if (_states.ContainsKey (request.ReminderId))
					return new Maybe<QueryResponse.CurrentReminderState> (_states [request.ReminderId]);
				else
					return Maybe<QueryResponse.CurrentReminderState>.Empty;
			}
		}
	}
}

