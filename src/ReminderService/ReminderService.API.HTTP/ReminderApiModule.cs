using System;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses;
using Nancy.Validation;
using ReminderService.Router;
using ReminderService.Messages;

namespace ReminderService.API.HTTP
{
	public class ReminderApiModule : NancyModule
	{
		private readonly IBus _bus;

		//todo: look at making the actions Async operations
		public ReminderApiModule (IBus bus) 
			: base("/reminders")
		{
			_bus = bus;

			Get ["/{reminderId}"] = parameters => {
				//handle getting a reminder with this ID...
				//parameters.reminderId....
				return this.Response.AsText("your reminder...");
			};

			Post["/"] = x => {
				var model = this.Bind<ScheduleReminder>();
				model.ReminderId = Guid.NewGuid();
				var result = this.Validate(model);

				if (!result.IsValid)
				{
					//return a 403 or something...
				}
					
				//errors are handled by an application level error handler, no need to try-catch here...
				_bus.Publish(model);
				return this.Response.AsJson(
					new ScheduledReminderResponse{ReminderId = model.ReminderId},
					HttpStatusCode.Created);
			};



			Delete ["/{reminderId}"] = parameters => {
				//cancel a reminder
				return 200;
			};
		}
	}
}

