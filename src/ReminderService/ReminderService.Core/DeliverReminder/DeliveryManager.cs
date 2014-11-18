using System;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Common;
using System.Threading.Tasks;

namespace ReminderService.Core.DeliverReminder
{
	public class DeliveryManager : IConsume<ReminderMessage.Due>
	{
		private const string ServiceInstanceId = "ForgetMeNot01";
		private readonly DeliveryRouter _router;
		private readonly IDeliveryStateManager _deliveryStateManager;

		public DeliveryManager (DeliveryRouter router, IDeliveryStateManager deliveryStateManager)
		{
			Ensure.NotNull (router, "router");
			Ensure.NotNull (deliveryStateManager, "deliveryStateManager");

			_router = router;
		}

		public void Handle (ReminderMessage.Due due)
		{
			_deliveryStateManager.Deliver (due);
		}
	}
}

