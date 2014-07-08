using System;
using Nancy;
using OpenTable.Services.Components.Logging;

namespace ReminderService.API.HTTP
{
	public class CustomBootstrapper : DefaultNancyBootstrapper
	{
		private static ILogger Logger = log4net.LogManager.GetLogger("ReminderService.API.HTTP");

		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			//define a custom error handler that will log all errors that occur when invoking a route.
			//keeps this noisy logging code (try-catch, etc...) out of the route handlers
			pipelines.OnError += (ctx, e) => {
				Logger.LogException(Level.Error, e, "Error processing request " + ctx.Request.Url);

				//do i need to return null here?
				return null;
			};
		}
	}
}

