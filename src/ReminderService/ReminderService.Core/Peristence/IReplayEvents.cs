using System;
using System.Collections.Generic;
using ReminderService.Common;
using ReminderService.Messages;

namespace ReminderService.Core.Persistence
{
	public interface IReplayEvents
	{
		IObservable<T> Replay<T>(DateTime from);
	}
}

