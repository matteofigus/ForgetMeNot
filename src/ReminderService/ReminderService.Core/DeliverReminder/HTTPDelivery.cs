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

		public HTTPDelivery (IRestClient restClient)
		{
			Ensure.NotNull (restClient, "restClient");

			_restClient = restClient;
		}

		public void Send(ReminderMessage.Due dueReminder)
		{
			Deliver(dueReminder, dueReminder.DeliveryUrl,
				(success) => {},
				(failed) => {
					//failed, try sending to dead message url
					Deliver(dueReminder, dueReminder.DeadLetterUrl,
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
				{RequestFormat = DataFormat.Json}
				.AddBody (Encoding.UTF8.GetString (dueReminder.Payload))
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

