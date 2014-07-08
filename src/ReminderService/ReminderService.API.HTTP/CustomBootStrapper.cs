using System;
using Nancy;

namespace ReminderService.API.HTTP
{
	public class CustomBootstrapper : DefaultNancyBootstrapper
	{
		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			//define a custom error handler that will log all errors that occur when invoking a route.
			//keeps this noisy logging code (try-catch, etc...) out of the route handlers
			pipelines.OnError += (ctx, Exception) => {
				//log
				return null;
			};
		}
	}
}

