using System;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Router.Tests.HandlingQueries
{
	public class TestRequest : IRequest<TestResponse>
	{
		public string CorrelationTag { get; set; }
	}

	public class TestResponse
	{
		public string CorrelationTag { get; set; }
	}

	//test: selects the right query handler

	public class TestRequest<T> : IRequest<T>
	{
		public string CorrelationTag { get; set; }
	}
}

