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
		IHandleQueries<RequestResponse.GetReminderState, Maybe<RequestResponse.CurrentReminderState>>
	{
		private readonly object lockObject = new object();
		private readonly Dictionary<Guid, RequestResponse.CurrentReminderState> _states = 
			new Dictionary<Guid, RequestResponse.CurrentReminderState>();

		public CurrentStateOfReminders ()
		{
		}

		public void Handle (Envelopes.Journaled<ReminderMessage.Schedule> envelope)
		{
			lock (lockObject) {
				if (!_states.ContainsKey (envelope.Message.ReminderId)) {
					_states.Add (envelope.Message.ReminderId, 
						new RequestResponse.CurrentReminderState (
							envelope.Message,
							RequestResponse.ReminderStatusEnum.Scheduled
						)
					);
				}
			}
		}

		public void Handle (Envelopes.Journaled<ReminderMessage.Cancel> envelope)
		{
			lock (lockObject) {
				if (_states.ContainsKey (envelope.Message.ReminderId))
					_states [envelope.Message.ReminderId].Status = RequestResponse.ReminderStatusEnum.Canceled;
			}
		}

		public void Handle (ReminderMessage.Delivered sent)
		{
			lock (lockObject) {
				if (_states.ContainsKey (sent.ReminderId))
					_states [sent.ReminderId].Status = RequestResponse.ReminderStatusEnum.Delivered;
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
					_states [undeliverable.ReminderId].Status = RequestResponse.ReminderStatusEnum.Undeliverable;
			}
		}

		public Maybe<RequestResponse.CurrentReminderState> Handle (RequestResponse.GetReminderState request)
		{
			lock (lockObject) {
				if (_states.ContainsKey (request.ReminderId))
					return new Maybe<RequestResponse.CurrentReminderState> (_states [request.ReminderId]);
				else
					return Maybe<RequestResponse.CurrentReminderState>.Empty;
			}
		}
	}
}

