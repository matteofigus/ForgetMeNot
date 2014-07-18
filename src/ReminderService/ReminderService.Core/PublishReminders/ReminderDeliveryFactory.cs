using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Router;
using ReminderService.Messages;
using RestSharp;

namespace ReminderService.Core.PublishReminders
{
	public static class ReminderDeliveryFactory
	{
		private static Func<HTTPPublisher> _httpInstanceFactory = () => {
			return null;
		};

		public static Func<HTTPPublisher> HttpDeliveryFactory {
			get { return _httpInstanceFactory; }
			set { _httpInstanceFactory = value; }
		}

		public static List<Func<ReminderMessage.Due, bool>> BuildHandlers()
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

					var handler = _httpInstanceFactory();
					handler.Send(due);
					return true;
				};
			}
		}
	}
}

