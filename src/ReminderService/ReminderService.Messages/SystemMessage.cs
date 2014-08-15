using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Messages
{
	public static class SystemMessage
	{
		public class Start : IMessage
		{
			//empty!
		}

		public class BeginInitialization : IMessage
		{
			//empty
		}

		public class InitializationCompleted : IMessage
		{
			//empty
		}

		public class ShutDown : IMessage
		{
			//empty!
		}
	}
}

