using System;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core.DeliverReminder
{
	public interface IDeliveryStateProvider
	{
		bool CheckAndLock(Guid reminderId, string serviceInstanceId);
	}

	public class DeliveryManager : IConsume<ReminderMessage.Due>
	{
		private const string ServiceInstanceId = "ForgetMeNot01";
		private readonly DeliveryRouter _router;
		private readonly IDeliveryStateProvider _deliveryStateProvider;

		public DeliveryManager (DeliveryRouter router)
		{
			Ensure.NotNull (router, "router");

			_router = router;
		}

		public void Handle (ReminderMessage.Due due)
		{
			//check the DB to see if this reminder has been locked to send
			if (_deliveryStateProvider.CheckAndLock (due.ReminderId, ServiceInstanceId)) {
				//if not, lock the row, set the row's service-name with this service instance id
				//_deliveryStateProvider.MarkAsDelivered (due.ReminderId, ServiceInstanceId);
				//send message to the router
				_router.Handle (due);
				//unlock the row
				_deliveryStateProvider.Unlock (due.ReminderId);
			}
		}
	}
}

