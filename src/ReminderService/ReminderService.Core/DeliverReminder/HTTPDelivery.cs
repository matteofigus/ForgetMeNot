using System;
using System.Text;
using RestSharp;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using log4net;
using System.Net;

namespace ReminderService.Core.DeliverReminder
{
	public class HTTPDelivery : IDeliverReminders
	{
		private readonly ILog Logger = LogManager.GetLogger(typeof(HTTPDelivery));
		private readonly IRestClient _restClient;

		public HTTPDelivery (IRestClient restClient)
		{
			Ensure.NotNull (restClient, "restClient");
			_restClient = restClient;
		}

		public void Send(ReminderMessage.Schedule dueReminder, string url, Action<ReminderMessage.Schedule> onSuccessfulSend, Action<ReminderMessage.Schedule, string> onFailedSend)
		{
			var request = new RestRequest (url, Method.POST)
			{ RequestFormat = DataFormat.Json }
				// since our payload is already valid JSON, we do not want to use the AddBody(...) method as this will JSONify our Json string and we get malformed Json as a result.
				// just add a body parameter directly to avoid this serializations step.
				.AddParameter ("application/json", Encoding.UTF8.GetString (dueReminder.Payload), ParameterType.RequestBody);

			_restClient.PostAsync (request, (response, handle) => {
				if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.Accepted)
					onSuccessfulSend(dueReminder);
				else if (response.ResponseStatus != ResponseStatus.Completed)
					onFailedSend(dueReminder, response.ErrorMessage);
				else
					onFailedSend(dueReminder, response.ErrorMessage + " - " + response.StatusDescription);
			});
		}
	}
}

