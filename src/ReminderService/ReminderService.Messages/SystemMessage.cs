using System;
using ReminderService.Router;

namespace ReminderService.Messages
{
	public static class SystemMessage
	{
		public class Start : IMessage
		{
			//empty!
		}

		public class ShutDown : IMessage
		{
			//empty!
		}
	}
}

