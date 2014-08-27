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
		private readonly ISendMessages _bus;

		public HTTPDelivery (IRestClient restClient, ISendMessages bus)
		{
			Ensure.NotNull (restClient, "restClient");
			Ensure.NotNull (bus, "bus");

			_restClient = restClient;
			_bus = bus;
		}

		public void Send(ReminderMessage.Schedule dueReminder, string url)
		{
			var req = new RestRequest (dueReminder.DeliveryUrl, Method.POST)
			{ RequestFormat = DataFormat.Json }
				// since our payload is already valid JSON, we do not want to use the AddBody(...) method as this will JSONify our Json string and we get malformed Json as a result.
				// just add a body parameter directly to avoid this serializations step.
				.AddParameter ("application/json", Encoding.UTF8.GetString (dueReminder.Payload), ParameterType.RequestBody);

			_restClient.PostAsync (req, (res, handle) => {
				if (res.StatusCode == HttpStatusCode.Created || res.StatusCode == HttpStatusCode.Accepted)
					_bus.Send(new ReminderMessage.Delivered(dueReminder.ReminderId, SystemTime.UtcNow()));
				else if (res.ResponseStatus != ResponseStatus.Completed)
					_bus.Send(new ReminderMessage.Undelivered(dueReminder, res.ErrorMessage));
				else
					_bus.Send(new ReminderMessage.Undelivered(dueReminder, res.StatusCode + " - " + res.StatusDescription));
			});
		}
	}
}

