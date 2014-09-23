using System;
using System.Collections.Generic;
using ReminderService.API.HTTP.BootStrap;
using ReminderService.Common;
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

namespace ReminderService.API.HTTP.Tests
{
	public class BusFactory : IBusFactory
	{
		const string DeadLetterUrl = "http://deadletter/url";
		private Bus _bus;
		private ITimer _timerInstance;
		private IRestClient _restClient;
		private IJournalEvents _journaler;
		private string _connectionString;
		private bool _overrideDeliveryHandlers = false;
		private List<Tuple<DeliveryTransport, IDeliverReminders>> _deliveryHandlers = new List<Tuple<DeliveryTransport, IDeliverReminders>> ();

		public BusFactory WithTimer (ITimer timer)
		{
			Ensure.NotNull (timer, "time");
			_timerInstance = timer;
			return this;
		}

		public BusFactory WithRestClient(IRestClient restClient)
		{
			Ensure.NotNull (restClient, "restClient");
			_restClient = restClient;
			return this;
		}

		public BusFactory WithDeliveryHandler(DeliveryTransport transport, IDeliverReminders handler)
		{
			Ensure.NotNull(handler, "handler");
			_deliveryHandlers.Add(Tuple.Create(transport, handler));
			_overrideDeliveryHandlers = true;
			return this;
		}

		public BusFactory WithJournaler(IJournalEvents journaler)
		{
			Ensure.NotNull (journaler, "journaler");
			_journaler = journaler;
			return this;
		}

		public BusFactory WithConnectionString(string connectionString)
		{
			Ensure.NotNullOrEmpty (connectionString, "connectionString");
			_connectionString = connectionString;
			return this;
		}

		public IBus Build()
		{
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

		private Journaler GetJournaler()
		{
			return new Journaler (_bus, _journaler);
		}

		private SystemStartManager GetStartupManager()
		{
			var replayers = new List<IReplayEvents> { 
				new CancellationReplayer(new PostgresCommandFactory(), _connectionString),
				new CurrentRemindersReplayer(new PostgresCommandFactory(), _connectionString),
			};
			var startManager = new SystemStartManager (_bus, replayers);
			return startManager;
		}

		private Scheduler GetScheduler()
		{
			var scheduler = new Scheduler (_bus, _timerInstance);
			return scheduler;
		}

		private CancellationFilter GetCancellationsHandler()
		{
			var httpDelivery = new HTTPDelivery (_restClient);
			var router = new DeliveryRouter (_bus, DeadLetterUrl);

			if (_overrideDeliveryHandlers)
				foreach (var handler in _deliveryHandlers) {
					router.AddHandler (handler.Item1, handler.Item2);
				}
			else {
				router.AddHandler (DeliveryTransport.HTTP, httpDelivery);
			}

			var cancellationFilter = new CancellationFilter (router);
			return cancellationFilter;
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
			//this will be configurable in service config...
			var httpDelivery = new HTTPDelivery (_restClient);
			return new DeadLetterDelivery (_bus, httpDelivery, DeadLetterUrl);
		}
	}
}

