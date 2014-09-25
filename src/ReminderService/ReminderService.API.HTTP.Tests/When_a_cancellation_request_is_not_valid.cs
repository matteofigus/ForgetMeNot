using NUnit.Framework;
using System;
using Nancy;
using Nancy.Testing;
using ReminderService.API.HTTP.Modules;

namespace ReminderService.API.HTTP.Tests
{
	public class When_a_cancellation_request_is_not_valid : ServiceSpec<ReminderApiModule>
	{
		[SetUp]
		public void When_the_reimderId_is_not_valid()
		{
			DELETE ("/reminders/", Guid.Empty);
		}

		[Test]
		public void Should_400()
		{
			Assert.AreEqual (HttpStatusCode.BadRequest, Response.StatusCode);
			Assert.IsTrue (
				Response.Body.AsString ().Contains (
					string.Format ("ReminderId [{0}] is not valid.", Guid.Empty)));
		}
	}
}

