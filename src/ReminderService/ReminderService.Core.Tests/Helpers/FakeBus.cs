﻿using System;
using ReminderService.Router;
using ReminderService.Common;

namespace ReminderService.Core.Tests
{
	public class FakeBus : IBus
	{
		private readonly Action<IMessage> _publishDelegate;

		public FakeBus ()
		{
			//empty
		}

		public FakeBus (Action<IMessage> publishDelegate)
		{
			_publishDelegate = publishDelegate;
		}

		public void Subscribe<T> (IConsume<T> handler) where T : class, IMessage
		{
			throw new NotImplementedException ();
		}

		public void UnSubscribe<T> (IConsume<T> handler) where T : IMessage
		{
			throw new NotImplementedException ();
		}

		public void Publish (IMessage message)
		{
			if (_publishDelegate != null)
				_publishDelegate (message);
		}
	}
}
