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
using ReminderService.Router.Consumers;

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

			// Get the current state of a reminder
			Get ["/{reminderId}"] = parameters => {
				//TODO: add a validator to make sure the reminderId is a valid GUID, so that we can do this parse with confidence that the resulting id is a GUID...
				Guid reminderId = Guid.Parse(parameters.reminderId.ToString());
			    var request = new RequestResponse.GetReminderState(reminderId);
                var response = bus.Send(request);

				if(response.HasValue)
					return Response.AsJson(response.Value);

				//serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

				return Response.AsJson(
					ErrorResponse.FromMessage(
						string.Format("Did not find reminder with Id [{0}]", reminderId)),
					HttpStatusCode.NotFound
				);
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
				_bus.Send(model);

				var scheduleRes = new ReminderMessage.ScheduledResponse{ReminderId = model.ReminderId};
				var res = Response.AsJson(
					scheduleRes,
					HttpStatusCode.Created);

				return res;
			};

			// Cancel a reminder
			Delete ["/{reminderId}"] = parameters => {
				Guid reminderId;
				bool parsed = Guid.TryParse(parameters.reminderId, out reminderId);
				if(!parsed || reminderId == Guid.Empty) {
					return Response.AsJson(
						ErrorResponse.FromMessage(
							string.Format("ReminderId [{0}] is not valid.", reminderId)), 
						HttpStatusCode.BadRequest);
				}

				//do we need to make sure that the reminderId exists and fail if it doesn't?
				//or can we just ignore the fact that the reminder does not exist?
				_bus.Send(new ReminderMessage.Cancel(reminderId));

				return HttpStatusCode.NoContent;
			};
		}

		public void Handle (SystemMessage.InitializationCompleted msg)
		{
			_systemHasInitialized = true;
		}
	}
}

