﻿using System;
using Nancy;

namespace ReminderService.API.HTTP.Modules
{
	public class RootModule : NancyModule
	{
		public RootModule ()
		{
			Get ["/"] = _ => {
				//do lbstatus in here
				return this.Response.AsText("OTWEB_ON");
			};
		}
	}
}

