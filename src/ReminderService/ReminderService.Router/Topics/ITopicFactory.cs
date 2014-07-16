using System;
using System.Collections.Generic;

namespace ReminderService.Router
{
	public interface ITopicFactory<T>
	{
		IList<string> GetTopicsFor (T item);
	}
}

