using System;
using NUnit.Framework;
using RestSharp;

namespace ReminderService.Test.Common
{
	[TestFixture]
	public class ExtensionTests
	{
		[Test]
		public void TestGetFakePayload ()
		{
			var guid = Guid.NewGuid();
			var fakePayload = new TestPayload(guid);
			IRestRequest request = new RestRequest();
			request.RequestFormat = DataFormat.Json;
			request.AddBody(fakePayload);

			var received = request.GetFakePayload();

			Assert.IsNotNull(received);
			Assert.AreEqual(guid, received.CorrelationId);
		}

		[Test]
		public void GetFakePayload_EmptyGuid()
		{
			var guid = Guid.Empty;
			var fakePayload = new TestPayload(guid);
			IRestRequest request = new RestRequest();
			request.RequestFormat = DataFormat.Json;
			request.AddBody(fakePayload);

			var received = request.GetFakePayload();

			Assert.IsNotNull(received);
			Assert.AreEqual(guid, received.CorrelationId);
		}

		[Test]
		public void GetFakePayload_NoRequestBody()
		{
			IRestRequest request = new RestRequest();
			request.RequestFormat = DataFormat.Json;

			var received = request.GetFakePayload();

			Assert.IsNull(received);
		}

		[Test]
		[ExpectedException(typeof(InvalidCastException))]
		public void GetFakePayload_BodyIsNotFakeRequest()
		{
			IRestRequest request = new RestRequest();
			request.RequestFormat = DataFormat.Json;
			request.AddBody (Guid.NewGuid ());

			var received = request.GetFakePayload();

			Assert.IsNull(received);
		}
	}
}

