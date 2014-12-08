using System;
using NUnit.Framework;
using ReminderService.Core.DeliverReminder;
using System.Threading.Tasks;

namespace ReminderService.Core.Tests.DeliverReminder
{
	[TestFixture]
	public class When_cannot_lock_record
	{
		private IDeliveryStateProvider _stateProvider;

		[Test]
		public void test()
		{

		}
	}

	public class FakeDeliveryStateProvider : IDeliveryStateProvider
	{
		private Action<Guid, string, bool> _getLockDelegate;

		public FakeDeliveryStateProvider (Action<Guid, string, bool> getLockDelegate)
		{
			_getLockDelegate = getLockDelegate;
		}

		public Task<bool> GetLock (Guid reminderId, string serviceInstanceId)
		{
			return _getLockDelegate (reminderId, serviceInstanceId);
		}

		public Task<bool> ReleaseLock (Guid reminderId, string serviceInstanceId)
		{
			throw new NotImplementedException ();
		}

	}
}

