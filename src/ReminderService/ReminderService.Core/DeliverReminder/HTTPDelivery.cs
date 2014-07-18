using System;
using System.Text;
using RestSharp;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using OpenTable.Services.Components.Logging;

namespace ReminderService.Core.DeliverReminder
{
	public class HTTPDelivery
	{
		private readonly ILogger _logger;
		private readonly IRestClient _restClient;
		private readonly IBus _bus;

		public HTTPDelivery (ILogger logger, IRestClient restClient, IBus bus)
		{
			Ensure.NotNull (logger, "logger");
			Ensure.NotNull (restClient, "restClient");
			Ensure.NotNull (bus, "bus");

			_logger = logger;
			_restClient = restClient;
			_bus = bus;
		}

		public void Send(ReminderMessage.Due dueReminder)
		{
			Deliver(dueReminder, dueReminder.DeliveryUrl,
				(success) => {
					_bus.Publish(dueReminder.AsSent());},
				(failed) => {
					//failed, try sending to dead message url
					Deliver(dueReminder, dueReminder.DeadLetterUrl,
						(success_dead) => {
							//we probably want to wrap the failed reminder in an envelope and include
							//any status codes, error information from the original send
						},
						(failed_dead) => {
							//if sending to the dead message url fails, what do we want to do?
							_logger.Log(Level.Error, "Unable to deliver reminder to any sepcified destination!");
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

