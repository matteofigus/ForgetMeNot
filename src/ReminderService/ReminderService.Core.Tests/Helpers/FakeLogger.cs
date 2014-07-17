using System;
using OpenTable.Services.Components.Logging;

namespace ReminderService.Core.Tests.Helpers
{
	public class FakeLogger : ILogger
	{
		private string _lastMessage;
		private readonly Action<Level, string> _logDelegate;

		public FakeLogger ()
		{
			//empty
		}

		public FakeLogger (Action<Level, string> logDelegate)
		{
			_logDelegate = logDelegate;
		}

		public string LastLoggedMessage {
			get { return _lastMessage; }
		}

		public void Configure (System.Collections.Generic.IDictionary<string, object> settings)
		{
			throw new NotImplementedException ();
		}

		public void Log (Level level, string message, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			_lastMessage = message;
			if(_logDelegate != null)
				_logDelegate (level, message);
		}

		public void Log (Level level, LogInfo logInfo, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			throw new NotImplementedException ();
		}

		public void LogException (Level level, Exception ex, string message, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			_lastMessage = message;
		}

		public void LogException (Level level, Exception ex, LogInfo logInfo, System.Collections.Generic.IDictionary<string, object> properties = null)
		{
			throw new NotImplementedException ();
		}
	}
}

