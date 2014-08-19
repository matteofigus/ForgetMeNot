using NUnit.Framework;
using System;
using ReminderService.Router.Tests.Helpers;
using ReminderService.Test.Common;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router.Tests
{
	[TestFixture]
	public class WhenSendingAQuery : Given_a_bus_instance
	{
		[TestFixtureSetUp]
		public void Initialize_Bus()
		{
			WithQueryHandler<TestRequest, TestResponse> (
				new FakeQueryHandler<TestRequest, TestResponse>(req =>
					new TestResponse{CorrelationTag = req.CorrelationTag}));
		}

		[Test]
		public void Should_receive_the_expected_response ()
		{
			var request = new TestRequest {CorrelationTag = "this is a query"};
			var response = Bus.Send (request);

            Assert.IsNotNull(response);
            Assert.AreEqual("this is a query", response.CorrelationTag);
		}
	}

	public class TestRequest : IRequest<TestResponse>
	{
		public string CorrelationTag { get; set; }
	}

	public class TestResponse
	{
		public string CorrelationTag { get; set; }
	}
}

