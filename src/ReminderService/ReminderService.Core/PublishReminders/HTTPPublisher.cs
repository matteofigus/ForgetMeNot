using System;
using RestSharp;
using ReminderService.Messages;

namespace ReminderService.Core
{
	public class HTTPPublisher
	{
		private readonly RestClient _restClient;

		public HTTPPublisher ()
		{
			_restClient = new RestClient ();
		}

		public void Send(ReminderMessage.Due dueReminder)
		{
			Deliver(dueReminder, dueReminder.DeliveryUrl,
				(success) => {/*do something if we succeed*/},
				(failed) => {
					//failed, try sending to dead message url
					Deliver(dueReminder, dueReminder.DeadLetterUrl,
						(success_dead) => {
							//we probably want to wrap the failed reminder in an envelope and include
							//any status codes, error information from the original send
						},
						(failed_dead) => {
							//if sending to the dead message url fails, what do we want to do?
						});
				});
		}

		private void Deliver(
			ReminderMessage.Due dueReminder, 
			string url, 
			Action<ReminderMessage.Due> onSuccess, 
			Action<ReminderMessage.Due> onFailed)
		{
			var req = new RestRequest (dueReminder.DeliveryUrl, Method.POST)
				.AddBody (dueReminder.Payload)
				.AddHeader ("content", dueReminder.ContentType);

			_restClient.PostAsync (req, (res, handle) => {
				if (res.ResponseStatus != ResponseStatus.Completed)
					onFailed (dueReminder);
				else
					onSuccess (dueReminder);
			});
		}
	}
}

