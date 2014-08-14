using System;

namespace ReminderService.Test.Common
{
	public class FakePayload
	{
		public Guid ReminderId { get; set; }

		public FakePayload ()
		{
			//empty
		}

		public FakePayload (Guid reminderId)
		{
			ReminderId = reminderId;
		}
	}
}

