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
using Nancy.TinyIoc;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenTable.Services.Components.Monitoring.Monitors.HitTracker;

namespace ReminderService.API.HTTP.BootStrap
{
	public class BootStrapper : DefaultNancyBootstrapper
	{
		private static readonly ILog Logger = LogManager.GetLogger("ReminderService.API.HTTP.Request");
		const string RequestTimerKey = "RequestTimer";
		const string RequestStartTimeKey = "RequestStartTime";

		protected override void ApplicationStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
		{
			base.ApplicationStartup (container, pipelines);

			var bus = container.Resolve<IBus> ();

			bus.Send (new SystemMessage.Start());

			//wait here for the SystemMessage.InititializationCompletedEvent ??
		}

		protected override void RequestStartup (Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, NancyContext context)
		{
			pipelines.OnError.AddItemToEndOfPipeline((ctx, ex) =>
				{
					container
						.Resolve<HitTracker>()
						.AppendHit(ctx.Request.Url.ToString(), new Hit {
							IsError = true,
							StartTime = SystemTime.UtcNow(),
							TimeTaken = TimeSpan.Zero
						});

					Logger.Error("Unhandled error on request: " + context.Request.Url + " : " + ex.Message, ex);
					return ErrorResponse.FromException(ex);
				});

			//response time
			pipelines.BeforeRequest.AddItemToStartOfPipeline (ctx => {
				ctx.Items[RequestTimerKey] = Stopwatch.StartNew();
				ctx.Items[RequestStartTimeKey] = SystemTime.UtcNow();
				return null;
			});

			pipelines.AfterRequest.AddItemToEndOfPipeline ((ctx) => {
				Maybe<DateTime> maybeStarted = MaybeGetItem<DateTime>(RequestStartTimeKey, ctx);
				Maybe<Stopwatch> maybeTimer = MaybeGetItem<Stopwatch>(RequestTimerKey, ctx);

				if(maybeTimer.HasValue) {
					var stopwatch = maybeTimer.Value;
					stopwatch.Stop();
					var elapsedMs = stopwatch.ElapsedMilliseconds;
					ctx.Items.Remove(RequestTimerKey);

					container
						.Resolve<HitTracker>()
						.AppendHit(ctx.Request.Url.ToString(), new Hit {
							StartTime = maybeStarted.HasValue ? maybeStarted.Value : DateTime.MinValue,
							IsError = false,
							TimeTaken = stopwatch.Elapsed
						});
				}
			});

			//request size
//			pipelines.BeforeRequest.AddItemToEndOfPipeline (ctx => {
//				container
//					.Resolve<IPushEvents>()
//					.Push (new MonitorEvent(ctx.ResolvedRoute + " " + ctx.Request.Method, SystemTime.UtcNow(), "RequestContentSize", ctx.Request.Headers.ContentLength));
//			    return null;
//			});

			base.RequestStartup(container, pipelines, context);
		}

		const string HttpMonitorKey = "HttpApiObservable";
		protected override void ConfigureApplicationContainer (Nancy.TinyIoc.TinyIoCContainer container)
		{
			base.ConfigureApplicationContainer (container);

			Logger.Info ("Configuring bus...");
			var busInstance = (Bus)container.Resolve<IBusFactory> ().Build ();
			container.Register<IBus, Bus> (busInstance);

			container.Register(typeof(JsonSerializer), typeof(CustomJsonSerializer));

			//TODO: get setttings from config
			var hitTracker = new HitTracker (HitTrackerSettings.Instance);
			container.Register<HitTracker> (hitTracker);
		}

		private static Maybe<T> MaybeGetItem<T>(string key, NancyContext context)
		{
			object requestStart;
			if (context.Items.TryGetValue (key, out requestStart))
				return new Maybe<T> ((T)requestStart);

			return Maybe<T>.Empty;
		}
	}
}

