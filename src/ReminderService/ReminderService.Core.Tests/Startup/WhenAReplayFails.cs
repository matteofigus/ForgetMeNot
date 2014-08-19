using NUnit.Framework;
using System;
using System.Reactive.Linq;
using System.Reactive;
using System.Linq;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Router;
using ReminderService.Messages;
using ReminderService.Core.Startup;
using ReminderService.Common;
using ReminderService.Core.Persistence;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Test.Common;
using System.Collections.Generic;

namespace ReminderService.Core.Tests.Startup
{
	[TestFixture]
	public class WhenAReplayFails : RoutableTestBase, IConsume<SystemMessage.InitializationCompleted>
	{
		[TestFixtureSetUp]
		public void When_the_system_starts()
		{
			_startupManager.Handle (new SystemMessage.Start());
		}

		[Test]
		public void Then_should_not_send_the_InitializationCompleted_event ()
		{
			Received.DoesNotContain <SystemMessage.InitializationCompleted>();
		}

		public void Handle (SystemMessage.InitializationCompleted msg)
		{
			Received.Add (msg);
		}

		private SystemStartManager _startupManager;

		public WhenAReplayFails ()
		{
			var cancellationReplayer = new FakeReplayer ((from) => 
				Observable
					.Range (0, 5)
					.Select (x => new ReminderMessage.Cancel (Guid.NewGuid ()))
					.Cast<IMessage> ());

			var currentRemindersReplayer = new FakeReplayer ((from) =>
				Observable.Throw<ReminderMessage.Schedule>(new Exception("Something went wrong in the database")));

			_startupManager = new SystemStartManager (Bus, new List<IReplayEvents>{cancellationReplayer, currentRemindersReplayer});
			Subscribe ((IConsume<SystemMessage.InitializationCompleted>)this);
		}
	}
}

