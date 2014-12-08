using System;
using System.Threading.Tasks;
using ReminderService.Messages;
using ReminderService.Common;

namespace ReminderService.Core.DeliverReminder
{
	public interface IDeliveryStateProvider
	{
		//bool CheckAndLock(Guid reminderId, string serviceInstanceId);

		Task<bool> GetLock (Guid reminderId, string serviceInstanceId);

		Task<bool> ReleaseLock (Guid reminderId, string serviceInstanceId);
	}

	public interface IDeliveryStateManager
	{
		void Deliver(ReminderMessage.IReminder reminder);
	}

	public class PostgresDeliveryStateManager : IDeliveryStateManager
	{
		const string ServiceInstanceId = "FMN01";
		private readonly IDeliveryStateProvider _deliveryStateProvider;
		private readonly ICommandFactory _commandFactory;

		public PostgresDeliveryStateManager (ICommandFactory commandFactory)
		{
			Ensure.NotNull (commandFactory, "commandFactory");

			_commandFactory = commandFactory;
		}

//		public void Deliver (ReminderMessage.IReminder reminder)
//		{
//			throw new NotImplementedException ();
//			//check the DB to see if this reminder has been locked to send
//			if (_deliveryStateProvider.CheckAndLock (due.ReminderId, ServiceInstanceId)) {
//				//if not, lock the row, set the row's service-name with this service instance id
//				//_deliveryStateProvider.MarkAsDelivered (due.ReminderId, ServiceInstanceId);
//				//send message to the router
//				_router.Handle (due);
//				//unlock the row
//				//_deliveryStateProvider.Unlock (due.ReminderId);
//			}
//		}

		public void Deliver(ReminderMessage.Due due)
		{
			_deliveryStateProvider.GetLock (due.ReminderId, ServiceInstanceId)
				.ContinueWith (lockTask => {
					if(lockTask.IsFaulted){
						//log
						_deliveryStateProvider.ReleaseLock(due.ReminderId, ServiceInstanceId);
						return;
					}

					if(lockTask.Result) {
						//we got a lock on the record, lets send it
						_router.Handle(due);
						_deliveryStateProvider.ReleaseLock(due.ReminderId, ServiceInstanceId)
							.ContinueWith(releaseTask => {
								//log
							});
					}
					else {
						//somebody else sent this reminder already
						//no need to release the lock since we never aquired it in the first places
					}
				});
		}

		private async void Deliver(ReminderMessage.Due due)
		{
			var task = _deliveryStateProvider.GetLock(due.ReminderId, ServiceInstanceId);
			await task;
			if (task.IsFaulted) {
				//log
				_deliveryStateProvider.ReleaseLock (due.ReminderId, ServiceInstanceId);
				return;
			}

			if (task.Result) {
				//we got a lock on the record, lets send it
				_router.Handle(due);
				await _deliveryStateProvider.ReleaseLock(due.ReminderId, ServiceInstanceId)
					.ContinueWith(releaseTask => {
						//log
					});
			}
		}

		private bool HasNotBeenSentYet(Guid reminderId, string serviceInstanceId)
		{
			var returned = _commandFactory
				.GetDueReminderStateCommand (reminderId, serviceInstanceId)
				.ExecuteReader (System.Data.CommandBehavior.SingleRow);

			return true;
		}
	}
}

