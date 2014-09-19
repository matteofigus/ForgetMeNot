using System;
using System.Net;
using System.Web.Routing;
using System.Reactive.Linq;

namespace ForgetMeNot.TestClient.Http
{
	//Taken from here: http://joseoncode.com/2011/06/17/event-driven-http-server-in-c-with-rx-and-httplistener/

	public class HttpServer : IObservable<HttpListenerContext>, IDisposable
	{
		private readonly HttpListener _listener;
		private readonly IObservable<HttpListenerContext> _stream;

		public HttpServer (string url)
		{
			_listener = new HttpListener ();
			_listener.Prefixes.Add (url);
			_listener.Start ();
			_stream = ObservableHttpContext ();
		}

		private IObservable<HttpListenerContext> ObservableHttpContext()
		{
			return Observable.Create<HttpListenerContext>(obs =>
				Observable.FromAsyncPattern<HttpListenerContext>(_listener.BeginGetContext,
					_listener.EndGetContext)()
				//.Select(c => new RequestContext(c.Request, c.Response))
				.Subscribe(obs))
					.Repeat()
					.Retry()
					.Publish()
					.RefCount();
		}

		public IDisposable Subscribe (IObserver<HttpListenerContext> observer)
		{
			return _stream.Subscribe (observer);
		}

		public void Dispose ()
		{
			_listener.Stop ();
		}
	}
}

