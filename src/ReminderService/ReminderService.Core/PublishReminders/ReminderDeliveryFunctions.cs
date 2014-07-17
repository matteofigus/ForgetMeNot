using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;
using RestSharp;

namespace ReminderService.Core.PublishReminders
{
	public static class ReminderDeliveryFunctions
	{
		public static List<Func<ReminderMessage.Due, bool>> Build()
		{
			return new List<Func<ReminderMessage.Due, bool>> {
				HttpHandler,
			};
		}

		public static Func<ReminderMessage.Due, bool> HttpHandler
		{
			get { return (due) => {
					if (!due.DeliveryUrl.ToUpper().StartsWith("HTTP"))
						return false;

					var handler = new HTTPPublisher(new RestClient());
					handler.Send(due);
					return true;
				};
			}
		}
	}
}

