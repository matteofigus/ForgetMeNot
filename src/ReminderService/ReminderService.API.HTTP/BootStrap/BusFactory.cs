using System;
using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Core.DeliverReminder;
using ReminderService.Router;
using ReminderService.Messages;

namespace ReminderService.API.HTTP.BootStrap
{
	public class BusFactory : IBusFactory
	{
		private Bus _bus;

		public IBus Build()
		{
			_bus = new Bus ();
			_bus.Subscribe (GetJournaler ());

			var scheduler = GetScheduler ();
			_bus.Subscribe ((IConsume<ReminderMessage.Schedule>)scheduler);
			_bus.Subscribe ((IConsume<SystemMessage.Start>)scheduler);
			_bus.Subscribe ((IConsume<SystemMessage.ShutDown>)scheduler);



			return _bus;
		}

		public Journaler GetJournaler()
		{
			return new Journaler (_bus, new InMemoryJournaler ());
		}

		public Scheduler GetScheduler()
		{
			var scheduler = new Scheduler (_bus, new ThreadingTimer ());
			return scheduler;
		}

		public CancelledRemindersManager GetCancellationsHandler()
		{
			return null;
		}


	}
}

