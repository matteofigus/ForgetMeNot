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

		public SystemStartManager (IBus bus, IEnumerable<IReplayEvents> replayers)
		{
			Ensure.NotNull (bus, "bus");
			Ensure.NotNull (replayers, "replayers");

			_bus = bus;
			_replayers = new List<IReplayEvents> (replayers);
		}

		public void Handle (SystemMessage.Start start)
		{
			// Merge the observable sequences from all the replayers in to one stream
			// Play that stream over the bus to initialize components
			// When all observable sequences have completed, then we send a message indicating that init is completed => start normal operation
			// If any of the child observables error, then the merged observable will error => we will not publish the init completed message => the system will not start
			Observable
				.Merge (_replayers.Select (r => r.Replay<IMessage> (SystemTime.Now ())))
				.Subscribe (
				Observer.Create<IMessage> (
					(message) => _bus.Publish (message), 								// OnNext
					(error) => Logger.Error (error),									//OnError
					() => _bus.Publish (new SystemMessage.InitializationCompleted ())	//OnCompleted
				));
		}
	}
}

