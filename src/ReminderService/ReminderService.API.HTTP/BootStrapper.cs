using System;
using Nancy;
using log4net;

namespace ReminderService.API.HTTP
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
		private static readonly ILog Logger = LogManager.GetLogger("ReminderService.API.HTTP.Request");

		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			//define a custom error handler that will log all errors that occur when invoking a route.
			//keeps this noisy logging code (try-catch, etc...) out of the route handlers
			pipelines.OnError += (ctx, e) => {
				Logger.ErrorFormat("There was an error processing the request to {0}", ctx.Request.Url);

				//do i need to return null here?
				return null;
			};
		}
	}
}

