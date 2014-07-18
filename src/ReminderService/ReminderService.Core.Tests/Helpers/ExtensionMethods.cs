using System;
using System.Text;
using RestSharp;

namespace ReminderService.Core.Tests.Helpers
{
	public static class ExtensionMethods
	{
		public static byte[] AsUtf8Encoding(this TestPayload testPayload)
		{
			var payloadJson = SimpleJson.SerializeObject(testPayload);
			return Encoding.UTF8.GetBytes (payloadJson);
		}
	}
}

