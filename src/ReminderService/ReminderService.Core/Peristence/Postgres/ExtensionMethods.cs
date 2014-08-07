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

		public static Maybe<T> MaybeGet<T>(this IDataRecord reader, string fieldName)
		{
			var raw = reader [fieldName];

			if (raw is DBNull)
				return Maybe<T>.Empty;

			return new Maybe<T> ((T)raw);
		}

		//hacky, because postgres know nothing about Guids and stores GUids as strings, so no native support in the driver
		public static Guid GetGuid(this IDataRecord reader, string fieldName)
		{
			var raw = reader [fieldName];

			if (raw is DBNull)
				return Guid.Empty;

			return Guid.ParseExact (raw.ToString(), "D");
		}

		public static string AsJson(this IMessage message)
		{
			return JsonConvert.SerializeObject (message);
		}
	}
}

