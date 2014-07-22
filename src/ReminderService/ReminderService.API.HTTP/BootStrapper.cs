using System;
using Nancy;
using log4net;

namespace ReminderService.API.HTTP
{
	public class Bootstrapper : DefaultNancyBootstrapper
	{
		private static readonly ILog Logger = LogManager.GetLogger("ReminderService.API.HTTP.Request");

		protected override void RequestStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
		{
			pipelines.OnError.AddItemToEndOfPipeline((z, a) =>
				{
					Logger.Error("Unhandled error on request: " + context.Request.Url + " : " + a.Message, a);
					return ErrorResponse.FromException(a);
				});

			base.RequestStartup(container, pipelines, context);
		}

//		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
//		{
//			pipelines.OnError += (ctx, e) => {
//				Logger.ErrorFormat("There was an error processing the request to {0}", ctx.Request.Url);
//			};
//
//			base.ApplicationStartup (container, pipelines);
//		}
	}
}

