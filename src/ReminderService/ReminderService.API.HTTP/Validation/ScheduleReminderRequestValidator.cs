using System;
using FluentValidation;
using ReminderService.Common;
using ReminderService.Messages;

namespace ReminderService.API.HTTP
{
	public class ScheduleReminderRequestValidator : AbstractValidator<ReminderMessage.Schedule>
	{
		public ScheduleReminderRequestValidator ()
		{
			RuleFor (request => request.DeliveryUrl).NotEmpty ();
			RuleFor (request => request.Payload).NotEmpty ();
			RuleFor (request => request.Payload).IsValidJson();
			RuleFor (request => request.ContentType)
				.NotEmpty ()
				.Equal ("application/json")
				.WithMessage ("We only support json at the moment.");
			RuleFor (request => request.DueAt)
				.GreaterThan (SystemTime.Now ())
				.WithMessage("The TimeoutAt value cannot be in the past.");
			RuleFor (request => request.MaxRetries)
				.GreaterThanOrEqualTo (0);
			RuleFor (request => request.GiveupAfter)
				.Must ((schedule, giveUp) =>
					giveUp.HasValue ? giveUp.Value > schedule.DueAt : true
				);
		}
	}
}

