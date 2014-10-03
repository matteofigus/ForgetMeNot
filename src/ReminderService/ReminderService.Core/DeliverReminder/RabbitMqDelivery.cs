using System;
using ReminderService.Messages;
using OpenTable.Services.Components.RabbitMq;

namespace ReminderService.Core.DeliverReminder
{
	public class RabbitMqDelivery : IDeliverReminders
	{
		private IMessagePublisher _messagePublisher;

		public RabbitMqDelivery(IMessagePublisher messagePublisher)
		{
			_messagePublisher = messagePublisher;
		}

		public void Send(ReminderMessage.Schedule dueReminder, string url, Action<ReminderMessage.Schedule> onSuccessfulSend, Action<ReminderMessage.Schedule, string> onFailedSend)
		{
			try
			{
				_messagePublisher.Connect(url);
			}
			catch (Exception ex) 
			{
				onFailedSend(dueReminder, ex.Message);
				return;
			}

			_messagePublisher.Publish(dueReminder.Payload, 
				new RoutingParameters { ContentEncoding = "utf-8", ContentType = "application/javascript" },
				() => { onSuccessfulSend(dueReminder); },
				ex => { onFailedSend(dueReminder, ex.Message); });
		}
	}
}

