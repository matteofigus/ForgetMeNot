using System;
using ReminderService.Router;

namespace ReminderService.API.HTTP.BootStrap
{
	public interface IBusFactory
	{
		IBus Build();
	}
}

