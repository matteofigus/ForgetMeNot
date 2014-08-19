using NUnit.Framework;
using System;
using ReminderService.Core.Startup;
using ReminderService.Core.Tests.Helpers;
using ReminderService.Messages;
using ReminderService.Router;
using ReminderService.Core.Persistence;
using ReminderService.Router.MessageInterfaces;
using ReminderService.Test.Common;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace ReminderService.Core.Tests.Startup
{
	[TestFixture]
	public class OnSuccessfulStartUp : 
		RoutableTestBase, 
		IConsume<SystemMessage.InitializationCompleted>, 
		IConsume<ReminderMessage.Cancel>, 
		IConsume<ReminderMessage.Schedule>
	{
		[TestFixtureSetUp]
		public void When_the_system_starts()
		{
			//_startupManager.Handle (new SystemMessage.Start ());
		}

		[Test]
		public void Then_should_send_InitializationCompleted_event ()
		{
			_startupManager.Handle (new SystemMessage.Start ());

			Assert.IsTrue (Received.ContainsOne<SystemMessage.InitializationCompleted> ());
			Assert.AreEqual (5, _cancellationCount);
			Assert.AreEqual (5, _scheduleCount);
		}



		private IObservable<IMessage> BuildCancelEventStream(int count)
		{
			return Observable
				.Range (0, count)
				.Select (x => new ReminderMessage.Cancel (Guid.NewGuid()))
				.Cast<IMessage> ();
		}

		private IObservable<IMessage> BuildCurrentReminderStream(int count)
		{
			return Observable
				.Range (0, count)
				.Select (x => new ReminderMessage.Schedule ())
				.Cast<IMessage> ();
		}

		public void Handle (SystemMessage.InitializationCompleted msg)
		{
			Received.Add (msg);
		}

		public void Handle (ReminderMessage.Cancel msg)
		{
			_cancellationCount++;
		}

		public void Handle (ReminderMessage.Schedule msg)
		{
			_scheduleCount++;
		}

		private SystemStartManager _startupManager;
		private int _cancellationCount = 0;
		private int _scheduleCount = 0;

		public OnSuccessfulStartUp ()
		{
			var cancellationReplayer = new FakeReplayer ((from) => this.BuildCancelEventStream(5));
			var currentRemindersReplayer = new FakeReplayer ((from) => this.BuildCurrentReminderStream(5));
			_startupManager = new SystemStartManager (Bus, new List<IReplayEvents>{cancellationReplayer, currentRemindersReplayer});
			Subscribe ((IConsume<SystemMessage.InitializationCompleted>)this);
			Subscribe ((IConsume<ReminderMessage.Cancel>)this);
			Subscribe ((IConsume<ReminderMessage.Schedule>)this);
		}
	}
}

