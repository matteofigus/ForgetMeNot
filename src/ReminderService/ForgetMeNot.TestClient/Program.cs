using System;
using System.Linq;
using System.Collections.Generic;
using NDesk.Options;
using System.Reactive.Linq;
using RestSharp;
using System.Text;

namespace ForgetMeNot.TestClient.Http
{
	class MainClass
	{
		static Uri _deliveryEndpoint;
		static Uri _forgetMeNotEndpoint;
		static string _filePath;
		static FileParser _fileParser;
		static RestClient _restClient;

		public static void Main (string[] args)
		{
			ParseArgs (args);

			Console.WriteLine ("ForgetMeNot Uri: " + _forgetMeNotEndpoint);
			Console.WriteLine ("Delivery Uri: " + _deliveryEndpoint);
			Console.WriteLine ("File: " + _filePath);

			//standup the http listener to receive due reminders
			using (var server = new HttpServer (_deliveryEndpoint.ToString ())) {
				Console.WriteLine (string.Format ("Listening for due reminders to be delivered on {0} ...", _deliveryEndpoint.ToString ()));
				server
					.Where (context => context.Request.HttpMethod == "POST")
					.Subscribe (context => context.Request.InputStream.ReadBytes (context.Request.ContentLength)
						.Subscribe (bytes => {
							Console.WriteLine ("Received: " + Encoding.UTF8.GetString (bytes));
							context.Respond (new EmptyResponse (201));
					})
				);
			
				server
					.Where (context => context.Request.HttpMethod != "POST")
					.Subscribe (context => {
						Console.WriteLine ("I can only process POST requests...received an HTTP " + context.Request.HttpMethod);
				});

				// read requests from the file and POST
				_fileParser = new FileParser (_filePath);
				_restClient = new RestClient (_forgetMeNotEndpoint.ToString ());
				foreach (var request in _fileParser.Parse()) {
					var response = _restClient.Post (
						              new RestRequest (Method.POST) 
							{ RequestFormat = DataFormat.Json, }
					.AddBody (request));

					if (response.StatusCode != System.Net.HttpStatusCode.Created)
						Console.WriteLine ("Could not schedule reminder: " + response.Content);

					Console.WriteLine ("Reminder Scheduled: " + response.Content);
				}

				Console.WriteLine ("Test running...");
				Console.ReadLine ();
				server.Dispose ();
			}
		}

		static void ParseArgs(string[] args)
		{
			bool show_help = false;
			var p = new OptionSet () {
				{ "d|deliveryUri=", "the Uri that will listen for delivery of reminders.",
					v => _deliveryEndpoint = ParseUriString(v) },
				{ "s|forgetmenot=", 
					"the Uri that the ForgetMeNot service is listening on to schedule reminders.",
					v => _forgetMeNotEndpoint = ParseUriString(v) },
				{ "f|file=", "the file that contains the reminders to schedule",
					v => _filePath = v },
				{ "h|help",  "show this message and exit", 
					v => show_help = v != null },
			};

			List<string> extra;
			try {
				extra = p.Parse (args);
			}
			catch (OptionException e) {
				Console.Write ("TestClient: ");
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try `--help' for more information.");
				return;
			}

			if (extra.Count > 2)
				ShowHelp (p);

			if (show_help) {
				ShowHelp (p);
				return;
			}
		}

		static Func<string, Uri> ParseUriString {
			get { 
				return (rawUri) => {
					Uri parsedUri;
					if (!Uri.TryCreate (rawUri, UriKind.Absolute, out parsedUri))
						throw new ArgumentException (string.Format ("[{0}] is not a valid Uri."));

					return parsedUri;
				};
			}
		}

		static void ShowHelp (OptionSet p)
		{
			Console.WriteLine ("Usage: ForgetMeNot.TestClient.Http OPTIONS");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			p.WriteOptionDescriptions (Console.Out);
		}
	}
}
