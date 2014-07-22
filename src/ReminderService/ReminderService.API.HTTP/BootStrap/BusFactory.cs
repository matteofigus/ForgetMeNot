using System;
using ReminderService.Core;
using ReminderService.Router;

namespace ReminderService.API.HTTP.BootStrap
{
	public class BusFactory : IBusFactory
	{
		private Bus _bus;

		public IBus Build()
		{
			_bus = new Bus ();
			_bus.Subscribe (GetJournaler ());



			return _bus;
		}

		public Journaler GetJournaler()
		{
			return new Journaler (_bus, new InMemoryJournaler ());
		}
	}
}

