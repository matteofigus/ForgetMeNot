using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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

			Post["/"] = parameters => {
				var model = this.Bind<ReminderMessage.Schedule>();
				model.ReminderId = Guid.NewGuid();
				var result = this.Validate(model);

				if (!result.IsValid)
				{
					var errors = result.Errors.Values.SelectMany(ee => ee.Select(e => ErrorResponse.FromMessage(e.ErrorMessage)));
					return Response.AsJson(errors, HttpStatusCode.BadRequest);
				}
					
				//errors are handled by an application level error handler, no need to try-catch here...
				_bus.Publish(model);

				var scheduleRes = new ReminderMessage.ScheduledResponse{ReminderId = model.ReminderId};
				var res = Response.AsJson(
					scheduleRes,
					HttpStatusCode.Created);

				return res;
			};



			Delete ["/{reminderId}"] = parameters => {
				//cancel a reminder
				return 200;
			};
		}
	}
}

