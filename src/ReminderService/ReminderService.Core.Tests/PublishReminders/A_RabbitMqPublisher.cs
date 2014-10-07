using System;
using NUnit.Framework;
using ReminderService.Common;
using ReminderService.Core.DeliverReminder;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Test.Common;
using TestPayload = ReminderService.Core.Tests.Helpers.TestPayload;

namespace ReminderService.Core.Tests.PublishReminders
{
	[TestFixture]
	public class When_a_RabbitMqPublisher_publishes_a_message
	{
		FakeRabbitMqPublisher _fakeRabbitMqPublisher;

		[SetUp]
		public void SetUp()
		{
			var payload = new TestPayload()
			{ 
				Property1 = "string property", 
				Property2 = 42, 
				Property3 = SystemTime.Now () 
			};

			var due = new ReminderMessage.Schedule (
				Guid.NewGuid(), 
				SystemTime.Now(), 
				"amqp://delivery/url",
				"content", 
				ReminderMessage.ContentEncodingEnum.utf8, 
				ReminderMessage.TransportEnum.rabbitmq,  
				payload.AsUtf8Encoding(), 
				0);

			_fakeRabbitMqPublisher = new FakeRabbitMqPublisher();

			var publisher = new RabbitMqDelivery(_fakeRabbitMqPublisher);

			//act
			publisher.Send(due, due.DeliveryUrl, null, null);
		}

		[Test]
		public void Should_connect_to_the_DeliveryUrl()
		{
			Assert.IsNotNull(_fakeRabbitMqPublisher.LastConnectionString);        
			Assert.AreEqual("amqp://delivery/url", _fakeRabbitMqPublisher.LastConnectionString);
		}   

		[Test]
		public void Should_publish_payload_with_fixed_encoding_and_contenttype()
		{
			Assert.IsNotNull(_fakeRabbitMqPublisher.LastRoutingParameters);        
			Assert.AreEqual("utf-8", _fakeRabbitMqPublisher.LastRoutingParameters.ContentEncoding);
			Assert.AreEqual("application/javascript", _fakeRabbitMqPublisher.LastRoutingParameters.ContentType);
		}
	}
}

