using System;
using System.Linq;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;
using ReminderService.Router;
using System.Reactive;
using System.Reactive.Linq;
using log4net;
using System.Threading.Tasks;
using System.Threading;
using ReminderService.Core.Persistence;

namespace ReminderService.Core.Startup
{
	public class SystemStartManager : IConsume<SystemMessage.Start>
	{
		private readonly static ILog Logger = LogManager.GetLogger(typeof(SystemStartManager));
		private readonly IBus _bus;
		private readonly List<IReplayEvents> _replayers;
		private readonly IReplayEvents _cancellationReplayer;
		private readonly IReplayEvents _remindersReplayer;

		public SystemStartManager (IBus bus, IEnumerable<IReplayEvents> replayers)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (replayers, "replayers");

			_bus = bus;
			_replayers = new List<IReplayEvents> (replayers);
		}

		public SystemStartManager (IBus bus, IReplayEvents cancellationReplayer, IReplayEvents remindersReplayer)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (cancellationReplayer, "cancellationReplayer");
			Ensure.NotNull (remindersReplayer, "remindersReplayer");

			_bus = bus;
			_cancellationReplayer = cancellationReplayer;
			_remindersReplayer = remindersReplayer;
		}

		public void Handle (SystemMessage.Start start)
		{
			RunStartTasks_MergeObservables2 (SystemTime.Now (), () => _bus.Publish (new SystemMessage.InitializationCompleted ()));
		}

		private void RunStartTasks_MergeObservables2(DateTime startFrom, Action onCompleted)
		{
			// Merge the observable sequences from all the replayers in to one stream
			// Play that stream over the bus to initialize components
			// When all observable sequences have completed, then we send a message indicating that init is completed => start normal operation
			// If any of the child observables error, then the merged observable will error => we will not publish the init completed message => the system will not start
			Observable
				.Merge(_replayers.Select (r => r.Replay<IMessage> (startFrom)))
				.Subscribe (
					Observer.Create<IMessage> (
						(message) => _bus.Publish (message),
						(error) => Logger.Error(error),
						() => onCompleted() //actually we want to publish a message indicating that all components have initialized
					));
		}

		private void RunStartTasks_AsyncTasks()
		{
			var replayFrom = SystemTime.Now ();
			var t1 = Task.Factory.StartNew (() => StartScheduler(replayFrom));
			var t2 = Task.Factory.StartNew ( () => StartCancellationFilter(replayFrom));

			Task.WaitAll (new []{ t1, t2 });
		}

		private void StartScheduler(DateTime startFrom)
		{
			//replay shceduler messages
			//await until we have played all the messages
			var observable = _remindersReplayer.Replay<ReminderMessage.Schedule> (startFrom);
			var observer = Observer.Create<ReminderMessage.Schedule> (
				(reminder) => _bus.Publish (reminder));
			observable.Subscribe (observer);
		}

		private void StartCancellationFilter(DateTime startFrom)
		{
			//replay cancellation messages
			//await until we have played all the cancellation messages
			var observable = _cancellationReplayer.Replay<ReminderMessage.Cancel> (startFrom);
			var observer = Observer.Create<ReminderMessage.Cancel> (
				(cancel) => _bus.Publish (cancel));
			observable.Subscribe (observer);
		}
	}
}

