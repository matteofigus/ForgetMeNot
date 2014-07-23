using System;
using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Core.DeliverReminder;
using ReminderService.Router;
using ReminderService.Messages;
using RestSharp;

namespace ReminderService.API.HTTP.BootStrap
{
	public class BusFactory : IBusFactory
	{
		private Bus _bus;

		public IBus Build()
		{
			_bus = new Bus ();

			var journaler = GetJournaler ();
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Schedule>);

			var scheduler = GetScheduler ();
			_bus.Subscribe (scheduler as IConsume<JournaledEnvelope<ReminderMessage.Schedule>>);
			_bus.Subscribe (scheduler as IConsume<SystemMessage.Start>);
			_bus.Subscribe (scheduler as IConsume<SystemMessage.ShutDown>);

			var cancellationFilter = GetCancellationsHandler ();
			_bus.Subscribe (cancellationFilter as IConsume<ReminderMessage.Due>);
			_bus.Subscribe (cancellationFilter as IConsume<ReminderMessage.Cancel>);

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
			var httpDelivery = new HTTPDelivery (new RestClient());
			var router = new DeliveryRouter ();
			router.AddHandler (DeliveryTransport.HTTP, httpDelivery);
			var cancellationFilter = new CancelledRemindersManager (router);
			return cancellationFilter;
		}
	}
}

