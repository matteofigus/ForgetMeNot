using System;
using FluentValidation;
using System.Collections.Generic;
using System.Text;

namespace ReminderService.API.HTTP
{
	public static class Extensions
	{
		public static IRuleBuilderOptions<T, TElement> IsValidJson<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder)
		{
			return ruleBuilder.SetValidator (new IsValidJsonValidator());
		}

		public static string AsString(this byte[] byteArray)
		{
			return Encoding.UTF8.GetString (byteArray);
		}

		public static byte[] AsUtf8EncodedByteArray(this string source)
		{
			return Encoding.UTF8.GetBytes (source);
		}
	}
}

