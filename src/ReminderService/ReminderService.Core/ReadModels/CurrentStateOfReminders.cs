using System;
using ReminderService.Router;
using ReminderService.Messages;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ReminderService.Common;

namespace ReminderService.Core.ReadModels
{
	public class CurrentStateOfReminders : 
		IConsume<JournaledEnvelope<ReminderMessage.Schedule>>,
		IConsume<JournaledEnvelope<ReminderMessage.Cancel>>,
		IConsume<ReminderMessage.Sent>,
		IHandleQueries<RequestResponse.GetReminderState, Maybe<RequestResponse.CurrentReminderState>>
	{
		private readonly object lockObject = new object();
		private readonly Dictionary<Guid, RequestResponse.CurrentReminderState> _states = 
			new Dictionary<Guid, RequestResponse.CurrentReminderState>();

		public CurrentStateOfReminders ()
		{
		}

		public void Handle (JournaledEnvelope<ReminderMessage.Schedule> envelope)
		{
			lock (lockObject) {
				if (!_states.ContainsKey (envelope.Message.ReminderId)) {
					_states.Add (envelope.Message.ReminderId, new RequestResponse.CurrentReminderState {
						Reminder = envelope.Message,
						Status = RequestResponse.ReminderStatusEnum.Scheduled
					});
				}
			}
		}

		public void Handle (JournaledEnvelope<ReminderMessage.Cancel> envelope)
		{
			lock (lockObject) {
				if (_states.ContainsKey (envelope.Message.ReminderId))
					_states [envelope.Message.ReminderId].Status = RequestResponse.ReminderStatusEnum.Canceled;
			}
		}

		public void Handle (ReminderMessage.Sent sent)
		{
			lock (lockObject) {
				if (_states.ContainsKey (sent.ReminderId))
					_states [sent.ReminderId].Status = RequestResponse.ReminderStatusEnum.Delivered;
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

