using System;
using Nancy;
using log4net;
using OpenTable.Services.Components.Logging;
using OpenTable.Services.Components.Logging.Log4Net;

namespace ReminderService.API.HTTP
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			//define a custom error handler that will log all errors that occur when invoking a route.
			//keeps this noisy logging code (try-catch, etc...) out of the route handlers
			//hmmm, I dont want this to log an entry against the BootStrapper type. I want it to log
			//against something like "ReminderService.API.HTTP.Request"
			pipelines.OnError += (ctx, e) => {
				ILogger logger = new BasicLogger<Bootstrapper>();
				logger.LogException(Level.Error, e, "Error processing request " + ctx.Request.Url);

				//do i need to return null here?
				return null;
			};
		}
	}
}

