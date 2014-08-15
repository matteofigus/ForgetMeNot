﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using ReminderService.Messages;
using RestSharp;

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

		public static FakePayload GetFakePayload(this ReminderMessage.Schedule reminder)
		{
			var serializer = new JavaScriptSerializer ();
			var payloadString = Encoding.UTF8.GetString (reminder.Payload);
			var fakePayload = serializer.Deserialize<FakePayload>(payloadString);
			return fakePayload;
		}
	}
}
