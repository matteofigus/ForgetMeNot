using NUnit.Framework;
using System;
using System.Linq;
using ReminderService.Messages;
using ReminderService.Common;
using ReminderService.Test.Common;
using RestSharp;
using Nancy;
using Nancy.Testing;
using System.Text;
using ReminderService.API.HTTP.Models;
using OpenTable.Services.Components.RabbitMq;

namespace ReminderService.API.HTTP.Tests
{
	public class When_scheduling_a_rabbitmq_reminder : ServiceSpec<ReminderApiModule>
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			FreezeTime ();

			var scheduleRequest = new ScheduleReminder (
				Now.Add(2.Hours()).ToString("o", System.Globalization.CultureInfo.InvariantCulture),
				"amqp://delivery",
				"application/json",
				"utf8",
				"rabbitmq",
				"{\"property1\": \"payload\"}".AsUtf8EncodedByteArray(),
				0,
				string.Empty
			);

			POST ("/reminders", scheduleRequest);
		}

		[Test]
		public void should_return_a_reminder_id()
		{
			Assert.AreEqual (HttpStatusCode.Created, Response.StatusCode);
			var responseBody = Response.Body.DeserializeJson<ScheduledResponse>();
			Assert.AreNotEqual (Guid.Empty, responseBody.ReminderId);
		}

		[Test]
		public void should_connect_to_RabbitMq_in_order_to_deliver_the_reminder_when_due()
		{
			AdvanceTimeBy (2.Hours());
			FireScheduler ();
			Assert.IsNotNull(LastInterceptedRabbitMqConnect);
			Assert.AreEqual("amqp://delivery", LastInterceptedRabbitMqConnect);
		}

		[Test]
		public void should_deliver_the_body_of_the_reminder_when_due()
		{
			AdvanceTimeBy (2.Hours());
			FireScheduler ();
			Assert.IsNotNull(LastInterceptedRabbitMqPublishBody);
			Assert.IsTrue("{\"property1\": \"payload\"}".AsUtf8EncodedByteArray().SequenceEqual(LastInterceptedRabbitMqPublishBody));
		}

		[Test]
		public void should_deliver_using_the_correct_routing_parameters_when_due()
		{
			AdvanceTimeBy (2.Hours());
			FireScheduler ();
			Assert.IsNotNull(LastInterceptedRabbitMqPublishRoutingParameters);
			Assert.AreEqual("utf-8", LastInterceptedRabbitMqPublishRoutingParameters.ContentEncoding);
			Assert.AreEqual("application/javascript", LastInterceptedRabbitMqPublishRoutingParameters.ContentType);
		}	
	}
}

