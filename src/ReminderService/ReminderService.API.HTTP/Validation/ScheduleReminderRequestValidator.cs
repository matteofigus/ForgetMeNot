using System;
using FluentValidation;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.API.HTTP.Models;
using System.Collections.Generic;
using System.Linq;

namespace ReminderService.API.HTTP
{
	public class ScheduleReminderRequestValidator : AbstractValidator<ScheduleReminder>
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
			RuleFor (request => request.Encoding)
				.NotEmpty ()
				.Equal ("utf8")
				.WithMessage ("We only support UTF8 encoding at the moment.");
			RuleFor (request => request.Transport)
				.NotEmpty ()
				.Must(BeAValidTransport)
				.WithMessage ("We only support Http and RabbitMQ transports at the moment.");
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

		private bool BeAValidTransport(string transport)
		{
			//doing this because I am getting strange unit test results.
			//all unit tests fail because arg: transport is being passed in as null, except for the tests that
			//are testing the Transport property.
			//Need to fix / figure this out!
			if (transport == null)
				return true;

			var input = transport.ToLower ();
			return (input == "http" || input == "rabbitmq");
		}
	}
}

