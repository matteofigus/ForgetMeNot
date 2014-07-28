using System;
using System.Collections.Generic;
using ReminderService.Router;
using ReminderService.API.HTTP.BootStrap;
using ReminderService.Core;
using ReminderService.Core.ScheduleReminder;
using ReminderService.Messages;
using ReminderService.Common;
using RestSharp;
using ReminderService.Core.DeliverReminder;

namespace ReminderService.API.HTTP.Tests
{
	public class BusFactory : IBusFactory
	{
		private Bus _bus;
		private ITimer _timerInstance;
		private IRestClient _restClient;
		private IJournaler _journaler;
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

		public BusFactory WithJournaler(IJournaler journaler)
		{
			Ensure.NotNull (journaler, "journaler");
			_journaler = journaler;
			return this;
		}

		public IBus Build()
		{
			_bus = new Bus ();

			var journaler = GetJournaler ();
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Schedule>);
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Cancel>);
			_bus.Subscribe (journaler as IConsume<ReminderMessage.Sent>);

			var scheduler = GetScheduler ();
			_bus.Subscribe (scheduler as IConsume<JournaledEnvelope<ReminderMessage.Schedule>>);
			_bus.Subscribe (scheduler as IConsume<SystemMessage.Start>);
			_bus.Subscribe (scheduler as IConsume<SystemMessage.ShutDown>);

			var cancellationFilter = GetCancellationsHandler ();
			_bus.Subscribe (cancellationFilter as IConsume<ReminderMessage.Due>);
			_bus.Subscribe (cancellationFilter as IConsume<ReminderMessage.Cancel>);

			return _bus;
		}

		private Journaler GetJournaler()
		{
			return new Journaler (_bus, _journaler);
		}

		private Scheduler GetScheduler()
		{
			var scheduler = new Scheduler (_bus, _timerInstance);
			return scheduler;
		}

		private CancellationFilter GetCancellationsHandler()
		{
			var httpDelivery = new HTTPDelivery (_restClient);
			var router = new DeliveryRouter (_bus);

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
			
	}
}

