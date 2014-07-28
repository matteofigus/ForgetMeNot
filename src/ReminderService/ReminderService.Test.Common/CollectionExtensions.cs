using System;
using System.Linq;
using System.Collections.Generic;

namespace ReminderService.Test.Common
{
	//inspiration taken from: https://github.com/EventStore/EventStore/blob/dev/src/EventStore.Core.Tests/Helpers/CollectionsExtensions.cs
	public static class CollectionExtensions
	{
		public static bool DoesNotContain<TMessage>(this IEnumerable<object> collection)
		{
			return collection.DoesNotContain<TMessage>(v => true);
		}

		public static bool DoesNotContain<TMessage>(this IEnumerable<object> collection, Predicate<TMessage> predicate)
		{
			return collection.ContainsThisMany<TMessage>(0, predicate);
		}

		public static bool ContainsOne<TMessage>(this IEnumerable<object> collection)
		{
			return collection.ContainsOne<TMessage>(v => true);
		}

		public static bool ContainsOne<TMessage>(this IEnumerable<object> collection, Predicate<TMessage> predicate)
		{
			return collection.ContainsThisMany<TMessage>(1, predicate);
		}

		public static bool ContainsThisMany<TMessage>(this IEnumerable<object> collection, int n)
		{
			return collection.ContainsThisMany<TMessage>(n, v => true);
		}

		public static bool ContainsThisMany<TMessage>(this IEnumerable<object> collection, int n, Predicate<TMessage> predicate)
		{
			return collection.OfType<TMessage>().Count(v => predicate(v)) == n;
		}
	}
}

