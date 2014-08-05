using System;
using System.Data;

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
	}
}

