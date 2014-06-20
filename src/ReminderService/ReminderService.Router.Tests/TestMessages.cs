using System;

namespace ReminderService.Router.Tests
{
	public static class TestMessages
	{
		public class TestMessage : IMessage
		{

		}

		public class ADerivedTestMessage : TestMessage
		{

		}

		public class NotDerivedTestMessage : IMessage
		{

		}
	}
}

