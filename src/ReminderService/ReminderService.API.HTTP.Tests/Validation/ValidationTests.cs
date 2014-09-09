using NUnit.Framework;
using System;
using FluentValidation.TestHelper;

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
	}
}

