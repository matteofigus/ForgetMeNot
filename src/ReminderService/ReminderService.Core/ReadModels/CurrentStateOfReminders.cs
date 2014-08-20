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
		IHandleQueries<Queries.GetReminder, Maybe<Responses.CurrentReminderState>>
	{
		private readonly object lockObject = new object();
		private readonly Dictionary<Guid, Responses.CurrentReminderState> _states = 
			new Dictionary<Guid, Responses.CurrentReminderState>();

		public CurrentStateOfReminders ()
		{
		}

		public void Handle (JournaledEnvelope<ReminderMessage.Schedule> envelope)
		{
			lock (lockObject) {
				if (!_states.ContainsKey (envelope.Message.ReminderId)) {
					_states.Add (envelope.Message.ReminderId, new Responses.CurrentReminderState {
						OriginalReminder = envelope.Message,
						Status = Responses.ReminderStatusEnum.Scheduled
					});
				}
			}
		}

		public void Handle (JournaledEnvelope<ReminderMessage.Cancel> envelope)
		{
			lock (lockObject) {
				if (_states.ContainsKey (envelope.Message.ReminderId))
					_states [envelope.Message.ReminderId].Status = Responses.ReminderStatusEnum.Canceled;
			}
		}

		public void Handle (ReminderMessage.Sent sent)
		{
			lock (lockObject) {
				if (_states.ContainsKey (sent.ReminderId))
					_states [sent.ReminderId].Status = Responses.ReminderStatusEnum.Delivered;
			}
		}

		public Maybe<Responses.CurrentReminderState> Handle (Queries.GetReminder request)
		{
			lock (lockObject) {
				if (_states.ContainsKey (request.ReminderId))
					return new Maybe<Responses.CurrentReminderState> (_states [request.ReminderId]);
				else
					return Maybe<Responses.CurrentReminderState>.Empty;
			}
		}
	}
}

