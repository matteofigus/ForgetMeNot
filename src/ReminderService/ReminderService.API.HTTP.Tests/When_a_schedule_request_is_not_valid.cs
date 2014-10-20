using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using ReminderService.API.HTTP.BootStrap;
using ReminderService.API.HTTP.Models;
using ReminderService.Common;
using ReminderService.Core.DeliverReminder;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Test.Common;
using RestSharp;
using ReminderService.API.HTTP.Modules;

namespace ReminderService.API.HTTP.Tests
{
	[TestFixture ()]
	public class When_a_schedule_request_is_not_valid : ServiceSpec<ReminderApiModule>
	{
		[Test]
		public void Should_return_a_400()
		{
			var request = new ScheduleReminder (
				DateTime.Now.AddDays (1).ToString(),
				"",
				"application/json",
				"utf8",
				"http",
				"payload".AsUtf8EncodedByteArray(),
				0,
				string.Empty
			);

			POST ("/reminders", request);

			Assert.AreEqual (HttpStatusCode.BadRequest, this.Response.StatusCode);
			Assert.That (ResponseBody.Contains("'Delivery Url' should not be empty."), Is.True);
		}
	}
}

