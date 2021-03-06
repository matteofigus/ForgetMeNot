﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ForgetMeNot.TestClient.Http
{
	public class ScheduleRequest
	{
		public string DueAt { get; set; }
		public string GiveupAfter { get; set; }
		public int MaxRetries { get; set; }
		public string DeliveryUrl { get; set; }
		public string ContentType { get; set; }
		public string Encoding { get; set; }
		public string Transport { get; set;}
		public byte[] Payload { get; set; }

		public ScheduleRequest ()
		{
			//empty
		}

		public ScheduleRequest (string dueAt, string deliveryUrl, string contentType, string encoding, string transport, byte[] payload, int maxRetries, string giveupAfter)
		{
			DueAt = dueAt;
			DeliveryUrl = deliveryUrl;
			ContentType = contentType;
			Encoding = encoding;
			Transport = transport;
			Payload = payload;
			MaxRetries = maxRetries;
			GiveupAfter = giveupAfter;
		}
	}

	public class ScheduleRequests
	{
		public IEnumerable<ScheduleRequest> Requests { get; set; }
	}
}

