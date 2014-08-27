using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Core.DeliverReminder;
using ReminderService.Router;
using ReminderService.Messages;
using RestSharp;
using ReminderService.Core.Startup;
using ReminderService.Core.Persistence;
using ReminderService.Core.Persistence.Postgres;
using System.Collections.Generic;

namespace ReminderService.API.HTTP.BootStrap
{
	public class BusFactory : IBusFactory
	{
		const string ConnectionString = "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";
		const string DeadLetterUrl = "http://deadletter/url";
		private Bus _bus;

		public IBus Build()
		{
			_bus = new Bus ();

			var startupManager = GetStartupManager ();
			_bus.Subscribe (startupManager as IConsume<SystemMessage.Start>);

			var journaler = GetJournaler ();
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Schedule>);
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Cancel>);
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Delivered>);

			var scheduler = GetScheduler ();
			_bus.Subscribe (scheduler as IConsume<Envelopes.Journaled<ReminderMessage.Schedule>>);
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

		public CancellationFilter GetCancellationsHandler()
		{
			var httpDelivery = new HTTPDelivery (new RestClient(), _bus);
			var router = new DeliveryRouter (_bus, DeadLetterUrl);
			router.AddHandler (DeliveryTransport.HTTP, httpDelivery);
			var cancellationFilter = new CancellationFilter (router);
			return cancellationFilter;
		}

		public SystemStartManager GetStartupManager()
		{
			var commandFactory = new PostgresCommandFactory ();
			var replayers = new List<IReplayEvents> {
				new CancellationReplayer (commandFactory, ConnectionString),
				new CurrentRemindersReplayer(commandFactory, ConnectionString),
			};
			return new SystemStartManager (_bus, replayers);
		}
	}
}

