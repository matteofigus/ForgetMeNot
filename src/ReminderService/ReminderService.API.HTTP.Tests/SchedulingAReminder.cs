﻿using NUnit.Framework;
using System;
using ReminderService.Messages;
using ReminderService.Common;
using RestSharp;
using Nancy;
using Nancy.Testing;
using System.Text;

namespace ReminderService.API.HTTP.Tests
{
	public class SchedulingAReminder : ServiceSpec<ReminderApiModule>
	{
		[SetUp]
		public void when_scheduling_a_reminder()
		{
			FreezeTime ();
			var scheduleRequest = new ReminderMessage.Schedule (
				"http://delivery",
				"http://deadletter",
				"application/json",
				Now.Add(2.Hours()),
				Encoding.UTF8.GetBytes ("payload")
			);

			POST ("/reminders", scheduleRequest);
		}

		[Test]
		public void should_return_a_reminder_id()
		{
			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			var responseBody = Response.Body.DeserializeJson<ReminderMessage.ScheduledResponse>();
			Assert.AreNotEqual (Guid.Empty, responseBody.ReminderId);
		}

		[Test]
		public void should_deliver_the_reminder_when_due()
		{
			AdvanceTimeBy (2.Hours());
			FireScheduler ();
			Assert.IsNotNull (DeliveryRequest);
		}
	}
}
