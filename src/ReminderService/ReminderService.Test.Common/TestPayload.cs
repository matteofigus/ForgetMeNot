using System;

namespace ReminderService.Test.Common
{
	public class TestPayload
	{
		public Guid CorrelationId { get; set; }

		public TestPayload ()
		{
			//empty
		}

		public TestPayload (Guid correlationId)
		{
			CorrelationId = correlationId;
		}
	}
}

