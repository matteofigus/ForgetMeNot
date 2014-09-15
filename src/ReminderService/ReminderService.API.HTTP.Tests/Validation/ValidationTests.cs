using NUnit.Framework;
using System;
using FluentValidation.TestHelper;
using ReminderService.Common;
using ReminderService.API.HTTP.Models;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class ValidationTests
	{
		private ScheduleReminderRequestValidator _validator;

		[SetUp]
		public void Given_a_validator()
		{
			_validator = new ScheduleReminderRequestValidator ();
		}

		[Test]
		public void should_not_error_on_valid_json()
		{
			var bytes = "{\"property1\": \"value1\", \"property2\": \"value2\"}".AsUtf8EncodedByteArray();
			_validator.ShouldNotHaveValidationErrorFor (request => request.Payload, bytes);
		}

		[Test]
		public void should_error_on_invalid_json()
		{
			var bytes = "{\"property1\": \"value1\", \"property2\": \"value2\"".AsUtf8EncodedByteArray();
			_validator.ShouldHaveValidationErrorFor (request => request.Payload, bytes);
		}

		[Test]
		public void should_error_on_empty_payload_field()
		{
			_validator.ShouldHaveValidationErrorFor (request => request.Payload, new byte[0]); 
		}

		[Test]
		public void should_validate_encoding()
		{
			_validator.ShouldHaveValidationErrorFor (request => request.Encoding, string.Empty);
			_validator.ShouldHaveValidationErrorFor (request => request.Encoding, "badencoding");
			_validator.ShouldNotHaveValidationErrorFor (request => request.Encoding, "utf8");
		}

		[Test]
		public void should_validate_the_transport()
		{
			_validator.ShouldHaveValidationErrorFor (request => request.Transport, string.Empty);
			_validator.ShouldHaveValidationErrorFor (request => request.Transport, "badtransport");
			_validator.ShouldNotHaveValidationErrorFor (request => request.Transport, "Http");
			_validator.ShouldNotHaveValidationErrorFor (request => request.Transport, "RabbitMq");
			_validator.ShouldNotHaveValidationErrorFor (request => request.Transport, "http");
			_validator.ShouldNotHaveValidationErrorFor (request => request.Transport, "rabbitmq");
		}

		[Test]
		public void should_validate_the_DueAt_field()
		{
			SystemTime.Set (new DateTime(2014, 9, 12, 18, 0, 0, DateTimeKind.Utc));
			_validator.ShouldNotHaveValidationErrorFor (request => request.DueAt, "2014-09-12T11:48:14.9728320-07:00"); //valid ISO date with timzone offset
			_validator.ShouldNotHaveValidationErrorFor (request => request.DueAt, "2014-09-12T18:48:14.9728320Z"); //valid ISO UTC date with NO timzone offset
			_validator.ShouldHaveValidationErrorFor (request => request.DueAt, "9/12/2014 12:00:00 AM"); // not valid ISO date
			_validator.ShouldHaveValidationErrorFor (request => request.DueAt, string.Empty);
			_validator.ShouldHaveValidationErrorFor (request => request.DueAt, "not a date");
			_validator.ShouldHaveValidationErrorFor (request => request.DueAt, "2014-09-12T10:48:14.9728320-07:00"); //valid ISO date with timezone offset in the past
			_validator.ShouldHaveValidationErrorFor (request => request.DueAt, "2014-09-12T16:48:14.9728320Z"); //valid ISO UTC date in the past
		}

		[Test]
		public void should_validate_the_GiveupAfter_field()
		{
			var testRequest = new ScheduleReminder (
				"2014-09-12T11:48:14.9728320-07:00",
				"http://delivery/url",
				"application/json",
				"utf8",
				"http",
				new byte[0],
				1,
				""
			);

			SystemTime.Set (new DateTime(2014, 9, 12, 18, 0, 0, DateTimeKind.Utc));

			testRequest.GiveupAfter = "2014-09-12T12:48:14.9728320-07:00"; //valid ISO date with timzone offset
			_validator.ShouldNotHaveValidationErrorFor (request => request.GiveupAfter, testRequest); 

			testRequest.GiveupAfter = "2014-09-12T19:48:14.9728320Z"; //valid ISO UTC date with NO timzone offset
			_validator.ShouldNotHaveValidationErrorFor (request => request.GiveupAfter, testRequest);

			testRequest.GiveupAfter = "9/12/2014 12:00:00 AM"; // not valid ISO date
			_validator.ShouldHaveValidationErrorFor (request => request.GiveupAfter, testRequest);

			testRequest.GiveupAfter = string.Empty; //empty is valid - means that the client does not want to retry delivery.
			_validator.ShouldNotHaveValidationErrorFor (request => request.GiveupAfter, testRequest); 

			testRequest.GiveupAfter = "not a date";
			_validator.ShouldHaveValidationErrorFor (request => request.GiveupAfter, testRequest);

			testRequest.GiveupAfter = "2014-09-12T10:48:14.9728320-07:00"; //valid ISO date with timezone offset in the past
			_validator.ShouldHaveValidationErrorFor (request => request.GiveupAfter, testRequest); 

			testRequest.GiveupAfter = "2014-09-12T16:48:14.9728320Z"; //valid ISO UTC date in the past
			_validator.ShouldHaveValidationErrorFor (request => request.GiveupAfter, testRequest); 
		}
	}
}

