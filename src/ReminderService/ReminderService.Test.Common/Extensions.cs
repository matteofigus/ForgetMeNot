using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using RestSharp;
using System.Web.Script.Serialization;

namespace ReminderService.Test.Common
{
	public static class Extensions
	{
		public static byte[] AsJsonEncoded(this FakePayload payload)
		{
			var serializer = new JavaScriptSerializer ();
			return Encoding.UTF8.GetBytes (
				serializer.Serialize (payload));
		}

		public static string AsJsonString(this byte[] byteArray)
		{
			return Encoding.UTF8.GetString (byteArray);
		}

		public static FakePayload GetFakePayload(this IRestRequest request)
		{
			if(!request.Parameters.Exists(p => p.Type == ParameterType.RequestBody))
				return null;

			var serializer = new JavaScriptSerializer ();
			var bodyString = request
				.Parameters
				.Where (p => p.Type == RestSharp.ParameterType.RequestBody)
				.FirstOrDefault ()
				.Value
				.ToString ();
			var json = serializer.Deserialize<FakePayload>(bodyString);

			return json;
		}
	}
}

