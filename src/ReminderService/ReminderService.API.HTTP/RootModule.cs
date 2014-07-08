using System;
using Nancy;

namespace ReminderService.API.HTTP
{
	public class RootModule : NancyModule
	{
		public RootModule ()
		{
			Get ["/"] = _ => {
				//do lbstatus in here
			};
		}
	}
}

