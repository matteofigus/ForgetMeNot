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
			RuleFor (request => request.DeadLetterUrl).NotEmpty ();
			RuleFor (request => request.Payload).NotEmpty ();
			RuleFor (request => request.ContentType)
				.NotEmpty ()
				.Equal ("application/json")
				.WithMessage ("We only support json at the moment.");
			RuleFor (request => request.TimeoutAt)
				.GreaterThan (SystemTime.Now ())
				.WithMessage("The TimeoutAt value cannot be in the past.");
		}
	}
}

