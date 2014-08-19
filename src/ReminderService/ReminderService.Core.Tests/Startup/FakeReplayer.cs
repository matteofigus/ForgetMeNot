using System;
using ReminderService.Core.Persistence;
using ReminderService.Router.MessageInterfaces;

namespace ReminderService.Core.Tests
{
	public class FakeReplayer : IReplayEvents
	{
		private readonly Func<DateTime, IObservable<IMessage>> _generator;

		public FakeReplayer (Func<DateTime, IObservable<IMessage>> generator)
		{
			_generator = generator;
		}

		public IObservable<IMessage> Replay<IMessage> (DateTime from)
		{
			return (IObservable<IMessage>)_generator (from);
		}
	}
}

