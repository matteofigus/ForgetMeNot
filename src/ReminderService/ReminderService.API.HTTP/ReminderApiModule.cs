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
	public class ReminderApiModule : NancyModule, IConsume<SystemMessage.InitializationCompleted>
	{
		private readonly IBus _bus;
		private bool _systemHasInitialized = false;

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

			// Schedule a reminder
			Post["/"] = parameters => {
				var model = this.Bind<ReminderMessage.Schedule>();
				model.ReminderId = Guid.NewGuid();
				var result = this.Validate(model);

				if (!result.IsValid) {
					var errors = result.Errors.Values.SelectMany(ee => ee.Select(e => ErrorResponse.FromMessage(e.ErrorMessage)));
					return Response.AsJson(errors, HttpStatusCode.BadRequest);
				}

				//errors are handled by a request level error handler, no need to try-catch here...
				_bus.Publish(model);

				var scheduleRes = new ReminderMessage.ScheduledResponse{ReminderId = model.ReminderId};
				var res = Response.AsJson(
					scheduleRes,
					HttpStatusCode.Created);

				return res;
			};

			// Cancel a reminder
			Delete ["/{reminderId}"] = parameters => {
				Guid reminderId;
				var parsed = Guid.TryParse(parameters.reminderId, out reminderId);
				if(!parsed || reminderId == Guid.Empty) {
					return Response.AsJson(
						ErrorResponse.FromMessage(
							string.Format("ReminderId [{0}] is not valid.", reminderId)), HttpStatusCode.BadRequest);
				}

				//do we need to make sure that the reminderId exists and fail if it doesn't?
				//or can we just ignore the fact that the reminder does not exist?
				_bus.Publish(new ReminderMessage.Cancel(reminderId));

				return HttpStatusCode.NoContent;
			};
		}

		public void Handle (SystemMessage.InitializationCompleted msg)
		{
			_systemHasInitialized = true;
		}
	}
}

