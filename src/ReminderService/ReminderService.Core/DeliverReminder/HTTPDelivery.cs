using System;
using System.Text;
using RestSharp;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using log4net;

namespace ReminderService.Core.DeliverReminder
{
	public class HTTPDelivery : IDeliverReminders
	{
		private readonly ILog Logger = LogManager.GetLogger(typeof(HTTPDelivery));
		private readonly IRestClient _restClient;
		private readonly string _deadLetterUrl;

		public HTTPDelivery (IRestClient restClient, string deadLetterUrl)
		{
			Ensure.NotNull (restClient, "restClient");
			Ensure.NotNullOrEmpty (deadLetterUrl, "deadLetterUrl");

			_restClient = restClient;
			_deadLetterUrl = deadLetterUrl;
		}

		public void Send(ReminderMessage.Due dueReminder)
		{
			Deliver(dueReminder, dueReminder.DeliveryUrl,
				(success) => {},
				(failed) => {
					//failed, try sending to dead message url
					Deliver(dueReminder, _deadLetterUrl,
						(success_dead) => {
							//we probably want to wrap the failed reminder in an envelope and include
							//any status codes, error information from the original send
						},
						(failed_dead) => {
							//if sending to the dead message url fails, what do we want to do?
							Logger.Error("Unable to deliver reminder to any specified destination!");
							throw new ReminderUndeliverableException<ReminderMessage.Due>(dueReminder);
						});
				});
		}

		private void Deliver(
			ReminderMessage.Due dueReminder, 
			string url, 
			Action<ReminderMessage.Due> onSuccess, 
			Action<ReminderMessage.Due> onFailed)
		{
			var req = new RestRequest (url, Method.POST)
				{ RequestFormat = DataFormat.Json }
				// since our payload is already valid JSON, we do not want to use the AddBody(...) method as this will JSONify our Json string and we get malformed Json as a result.
				// just add a body parameter directly to avoid this serializations step.
				.AddParameter ("application/json", Encoding.UTF8.GetString (dueReminder.Payload), ParameterType.RequestBody);

			_restClient.PostAsync (req, (res, handle) => {
				if (res.ResponseStatus != ResponseStatus.Completed)
					onFailed (dueReminder);
				else
					onSuccess (dueReminder);
			});
		}
	}
}

