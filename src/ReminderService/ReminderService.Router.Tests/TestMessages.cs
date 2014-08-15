using ReminderService.Router.MessageInterfaces;


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

		public class SiblingOfADerivedTestMessage : TestMessage
		{

		}

		public class NotDerivedTestMessage : IMessage
		{

		}

		public class NotATestMessage
		{

		}
	}
}

