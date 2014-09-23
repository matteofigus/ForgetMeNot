using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ForgetMeNot.TestClient.Http
{
    public class Response
    {
        public Response()
        {
            WriteStream = s => { };
            StatusCode = 200;
            Headers = new Dictionary<string, string>{{"Content-Type", "text/html"}};
        }

        public int StatusCode { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public Action<Stream> WriteStream { get; set; }
    }

    public class StringResponse : Response
    {
        public StringResponse(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            WriteStream = s => s.Write(bytes, 0 , bytes.Length);
        }
    }

    public class EmptyResponse : Response
    {
        public EmptyResponse(int statusCode = 204)
        {
            StatusCode = statusCode;

        }
    }
}