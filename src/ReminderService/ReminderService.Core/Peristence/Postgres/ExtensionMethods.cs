using System;
using System.Data;
using Newtonsoft.Json;
using ReminderService.Common;

namespace ReminderService.Core
{
	public static class ExtensionMethods
	{
		//use automapper?
		//use dapper?
		public static T Get<T>(this IDataRecord reader, string fieldName, T defaultIfNull = default(T))
		{
			var raw = reader [fieldName];

			if (raw is DBNull) {
				if (defaultIfNull == null)
					return default(T);
				else
					return defaultIfNull;
			}

			return (T)raw;
		}

		public static string AsJson(this IMessage message)
		{
			return JsonConvert.SerializeObject (message);
		}
	}
}

