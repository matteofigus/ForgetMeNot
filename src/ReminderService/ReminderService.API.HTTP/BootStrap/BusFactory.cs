using System.Collections.Generic;
using System.Configuration;
using ReminderService.Core;
using ReminderService.Core.DeliverReminder;
using ReminderService.Core.Persistence;
using ReminderService.Core.Persistence.Postgres;
using ReminderService.Core.ReadModels;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Core.Startup;
using ReminderService.Messages;
using ReminderService.Router;
using RestSharp;

namespace ReminderService.API.HTTP.BootStrap
{
	public class BusFactory : IBusFactory
	{
		private string ConnectionString; //= "Server=127.0.0.1;Port=5432;Database=reminderservice;User Id=reminder_user;Password=reminder_user;";
		private string DeadLetterUrl; //= "http://deadletter/url";
		private Bus _bus;

		public IBus Build()
		{
			ConnectionString = ConfigurationManager.ConnectionStrings ["postgres"].ConnectionString;
			DeadLetterUrl = ConfigurationManager.ConnectionStrings ["deadletterqueue"].ConnectionString;

			_bus = new Bus ();

			var journaler = GetJournaler ();
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Schedule>);
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Cancel>);
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Delivered>);
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Undeliverable>);

			var scheduler = GetScheduler ();
			_bus.Subscribe (scheduler as IConsume<Envelopes.Journaled<ReminderMessage.Schedule>>);
			_bus.Subscribe (scheduler as IConsume<ReminderMessage.Rescheduled>);
			_bus.Subscribe (scheduler as IConsume<SystemMessage.Start>);
			_bus.Subscribe (scheduler as IConsume<SystemMessage.ShutDown>);

			var cancellationFilter = GetCancellationsHandler ();
			_bus.Subscribe (cancellationFilter as IConsume<ReminderMessage.Due>);
			_bus.Subscribe (cancellationFilter as IConsume<ReminderMessage.Cancel>);

			var startupManager = GetStartupManager ();
			_bus.Subscribe (startupManager as IConsume<SystemMessage.Start>);

			var currentReminderState = GetCurrentStateOfReminders ();
			_bus.Subscribe (currentReminderState);
			_bus.Subscribe (currentReminderState as IConsume<Envelopes.Journaled<ReminderMessage.Schedule>>);
			_bus.Subscribe (currentReminderState as IConsume<Envelopes.Journaled<ReminderMessage.Cancel>>);
			_bus.Subscribe (currentReminderState as IConsume<ReminderMessage.Delivered>);
			_bus.Subscribe (currentReminderState as IConsume<ReminderMessage.Undelivered>);
			_bus.Subscribe (currentReminderState as IConsume<ReminderMessage.Undeliverable>);

			var undeliveredProcessManager = GetUndeliverableRemindersProcessManager ();
			_bus.Subscribe (undeliveredProcessManager as IConsume<ReminderMessage.Delivered>);
			_bus.Subscribe (undeliveredProcessManager as IConsume<ReminderMessage.Undelivered>);

			var deadLetterDeliver = GetDeadLetterDelivery ();
			_bus.Subscribe (deadLetterDeliver as IConsume<ReminderMessage.Undeliverable>);

			return _bus;
		}

		public Journaler GetJournaler()
		{
			return new Journaler (_bus, new PostgresJournaler (new PostgresCommandFactory(), ConnectionString));
		}

		public Scheduler GetScheduler()
		{
			var scheduler = new Scheduler (_bus, new ThreadingTimer ());
			return scheduler;
		}

		public CancellationFilter GetCancellationsHandler()
		{
			var httpDelivery = new HTTPDelivery (new RestClient());
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

		private CurrentStateOfReminders GetCurrentStateOfReminders()
		{
			return new CurrentStateOfReminders ();
		}

		private UndeliveredProcessManager GetUndeliverableRemindersProcessManager()
		{
			return new UndeliveredProcessManager (_bus);
		}

		private DeadLetterDelivery GetDeadLetterDelivery()
		{
			var httpDelivery = new HTTPDelivery (new RestClient());
			return new DeadLetterDelivery (_bus, httpDelivery, DeadLetterUrl);
		}
	}
}

