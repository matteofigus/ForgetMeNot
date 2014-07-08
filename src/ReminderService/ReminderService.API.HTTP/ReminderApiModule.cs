using System;
using Nancy;
using ReminderService.Router;

namespace ReminderService.API.HTTP
{
	public class ReminderApiModule : NancyModule
	{
		private readonly IPublish _bus;

		//todo: look at making the actions Async operations
		public ReminderApiModule (IPublish bus) 
			: base("/reminders")
		{
			_bus = bus;

			Get ["/{reminderId}"] = parameters.reminderId => {
				//handle getting a reminder with this ID...
			};

			Put["/"] = reminder => {
				//handle scheduling
			};

			Delete["/{reminderId}"] = parameters.reminderId => {
				//cancel a reminder
			};
		}
	}
}

