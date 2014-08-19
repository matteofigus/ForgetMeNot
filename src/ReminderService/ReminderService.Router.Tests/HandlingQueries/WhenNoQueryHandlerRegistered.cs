using NUnit.Framework;
using System;
using ReminderService.Router.Tests.Helpers;
using ReminderService.Router.Tests.HandlingQueries;

namespace ReminderService.Router.Tests.HandlingQueries
{
	[TestFixture]
	public class WhenNoQueryHandlerRegistered : Given_a_bus_instance
	{
		[Test]
		public void Should_return_default_for_TResponse()
		{
			var request = new TestRequest {CorrelationTag = "this is a query"};
			var response = Bus.Send (request);

			Assert.IsNull (response);
		}
	}
}

