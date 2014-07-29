using NUnit.Framework;
using System;
using FluentValidation.TestHelper;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture]
	public class JsonValidator
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
	}
}

