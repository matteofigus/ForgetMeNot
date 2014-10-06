using System;
using System.Collections.Generic;
using OpenTable.Services.Components.RabbitMq;

namespace ReminderService.Test.Common
{
	public class FakeRabbitMqPublisher : IMessagePublisher
	{
		public Dictionary<string, string> Configuration { get; set; }
		public ConnectionParameters LastConnectionParameters { get; set; }
		public string LastConnectionString { get; set; }
		public string LastMessage { get; set; }
		public byte[] LastMessageBody { get; set; }
		public RoutingParameters LastRoutingParameters { get; set; }

		#region IMessagePublisher implementation

		public void Configure(Dictionary<string, string> settings)
		{
			Configuration = settings;
		}

		public void Connect(ConnectionParameters connectionParameters)
		{
			LastConnectionParameters = connectionParameters;
		}

		public void Connect(string connectionString, bool validate = false)
		{
			LastConnectionString = connectionString;
		}

		public void PublishUnconfirmed(string message, RoutingParameters routingParameters = null)
		{
			LastMessage = message;
			LastRoutingParameters = routingParameters;
		}

		public void PublishUnconfirmed(byte[] messageBody, RoutingParameters routingParameters = null)
		{
			LastMessageBody = messageBody;
			LastRoutingParameters = routingParameters;
		}

		public void Publish(string message, RoutingParameters routingParameters, Action onSuccess, Action<Exception> onFailure)
		{
			LastMessage = message;
			LastRoutingParameters = routingParameters;
		}

		public void Publish(byte[] messageBody, RoutingParameters routingParameters, Action onSuccess, Action<Exception> onFailure)
		{
			LastMessageBody = messageBody;
			LastRoutingParameters = routingParameters;
		}

		public void Disconnect()
		{
		}

		public Action<RecoverablePublishException, MessagingProperties> HandleRecoverableError
		{
			set
			{
			}
		}

		#endregion
	}
}

