using System;
using Nancy;
using log4net;
using ReminderService.Router;
using ReminderService.API.HTTP.BootStrap;
using ReminderService.Messages;
using ReminderService.Core.Startup;
using ReminderService.Core.Persistence;
using System.Collections.Generic;
using ReminderService.Core.Persistence.Postgres;
using Newtonsoft.Json;
using System.Diagnostics;
using ReminderService.Common;
using ReminderService.API.HTTP.Monitoring;

namespace ReminderService.API.HTTP.BootStrap
{
	public class BootStrapper : DefaultNancyBootstrapper
	{
		private static readonly ILog Logger = LogManager.GetLogger("ReminderService.API.HTTP.Request");
		const string RequestTimerKey = "RequestTimer";

		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			base.ApplicationStartup (container, pipelines);

			var bus = container.Resolve<IBus> ();

			bus.Send (new SystemMessage.Start());

			//wait here for the SystemMessage.InititializationCompletedEvent ??
		}

		protected override void RequestStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
		{
			pipelines.OnError.AddItemToEndOfPipeline((z, a) =>
				{
					Logger.Error("Unhandled error on request: " + context.Request.Url + " : " + a.Message, a);
					return ErrorResponse.FromException(a);
				});

			//response time
			pipelines.BeforeRequest.AddItemToStartOfPipeline (ctx => {
				ctx.Items[RequestTimerKey] = Stopwatch.StartNew();
				return null;
			});

			pipelines.AfterRequest.AddItemToEndOfPipeline ((ctx) => {
				object timer;
				if(ctx.Items.TryGetValue(RequestTimerKey, out timer)) {
					//todo: wrap the timer and event generation in a class; reuse the timer intance, and reuse the class instance between requests
					var stopwatch = (Stopwatch)timer;
					stopwatch.Stop();
					var elapsedMs = stopwatch.ElapsedMilliseconds;
					ctx.Items.Remove(RequestTimerKey);

					container
						.Resolve<ObservableMonitor<MonitorEvent>>()
						.PushEvent(new MonitorEvent(ctx.ResolvedRoute + " " + ctx.Request.Method, SystemTime.UtcNow(), "ResponseTime", elapsedMs));

					//IBus bus;
					//if(container.TryResolve<IBus>(out bus)) {}
						//bus.Send(new MonitorEvent(ctx.ResolvedRoute + " " + ctx.Request.Method, SystemTime.UtcNow(), "ResponseTime", elapsedMs));
				}
			});

			//request size
			pipelines.BeforeRequest.AddItemToEndOfPipeline (ctx => {
				container
					.Resolve<ObservableMonitor<MonitorEvent>>()
					.PushEvent(new MonitorEvent(ctx.ResolvedRoute + " " + ctx.Request.Method, SystemTime.UtcNow(), "RequestContentSize", ctx.Request.Headers.ContentLength));

				//IBus bus;
				//if(container.TryResolve<IBus>(out bus)) {
					//check if the content length header exists
					//bus.Send(new MonitorEvent(ctx.ResolvedRoute + " " + ctx.Request.Method, SystemTime.UtcNow(), "RequestContentSize", ctx.Request.Headers.ContentLength));
				//}
			    return null;
			});

			base.RequestStartup(container, pipelines, context);
		}


		protected override void ConfigureApplicationContainer (Nancy.TinyIoc.TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer (container);

			Logger.Info ("Configuring bus...");
			var busInstance = (Bus)container.Resolve<IBusFactory> ().Build ();
			container.Register<IBus, Bus> (busInstance);

			container.Register(typeof(JsonSerializer), typeof(CustomJsonSerializer));

			var observable = new ObservableMonitor<MonitorEvent> ();
			container.Register (observable);
			container.Register<HttpApiMonitor> (new HttpApiMonitor(observable, 1000, 10));
		}
	}
}

