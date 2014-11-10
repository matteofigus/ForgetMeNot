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
using ReminderService.API.HTTP.Models;

namespace ReminderService.API.HTTP.Modules
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
				Guid reminderId;
				if(!Guid.TryParse(parameters.reminderId.ToString(), out reminderId)){
					var res = Response.AsText("Not a valid Reminder Id.");
					res.StatusCode = HttpStatusCode.BadRequest;
					return res;
				}
					
			    var request = new QueryResponse.GetReminderState(reminderId);
                var response = bus.Send(request);

				if(response.HasValue)
					return Response.AsJson(new ReminderStatus(response.Value.Reminder, response.Value.Status.ToString(), response.Value.RedeliveryAttempts));

				return Response.AsJson(
					ErrorResponse.FromMessage(
						string.Format("Did not find reminder with Id [{0}]", reminderId)),
					HttpStatusCode.NotFound
				);
			};

			// Schedule a reminder
			Post["/"] = parameters => {
				var model = this.Bind<ScheduleReminder>();
				var result = this.Validate(model);

				if (!result.IsValid) {
					var errors = result.Errors.Values.SelectMany(ee => ee.Select(e => ErrorResponse.FromMessage(e.ErrorMessage)));
					return Response.AsJson(errors, HttpStatusCode.BadRequest);
				}

				var schedule = model.BuildScheduleMessage(Guid.NewGuid());
				if(parameters.replicated == "true")
					_bus.Send(new ClusterMessage.Replicate<ReminderMessage.Schedule>(schedule));
				else
					_bus.Send(schedule);

				var scheduleRes = new ScheduledResponse{ReminderId = schedule.ReminderId};
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

				var cancel = new ReminderMessage.Cancel(reminderId);
				if(parameters.replicated == "true")
					_bus.Send(new ClusterMessage.Replicate<ReminderMessage.Cancel>(cancel));
				else
					//do we need to make sure that the reminderId exists and fail if it doesn't?
					//or can we just ignore the fact that the reminder does not exist?
					_bus.Send(cancel);

				return HttpStatusCode.NoContent;
			};
		}

		public void Handle (SystemMessage.InitializationCompleted msg)
		{
			_systemHasInitialized = true;
		}
	}
}

