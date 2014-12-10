using System;
using Nancy.Hosting.Self;
using OpenTable.Services.Components.DiscoveryClient;
using ReminderService.API.HTTP.BootStrap;
using log4net;
using log4net.Config;
using System.Configuration;
using System.Reflection;
using System.Linq;
using System.IO;
using ReminderService.Common;
using System.Threading;
using NDesk.Options;
using NDesk.Options.Extensions;
using System.Collections.Generic;

namespace ReminderService.Hosting.NancySelf
{
	class MainClass
	{
		const string ServiceName = "forgetmenot";
		private static string _hostName;
		private static int _port;
		private static string _instanceId;
		private static string _environment;

		private static string _hostUri;
		private static ILog Logger = LogManager.GetLogger("ForgetMeNot.SelfHosted.Host");
		private static CSDiscoveryClient _discoveryClient;
		private static IAnnouncementLease _lease;
		private static bool _useDiscovery = true;
		private static ManualResetEvent _resetEvent = new ManualResetEvent (false);

		public static void Main (string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += (object sender, UnhandledExceptionEventArgs e) => {
				Logger.Error ("Unhandled exception within the AppDomain.", e.ExceptionObject as Exception);
			};

			if (!ParseArgs (args)) {
				Logger.Error ("Failed to start the service. Exiting...");
				return;
			}

			XmlConfigurator.Configure ();
			OTEnvironmentalConfigManager.SetEnvironment (_environment);

			Logger.Info ("Starting ForgetMeNot service...");

			_hostUri = string.Format ("http://{0}:{1}", _hostName, _port);
			Console.WriteLine ("Host URI: " + _hostUri);

			var hostSettings = new HostConfiguration ();
			hostSettings.UnhandledExceptionCallback = ex => {
				Logger.Error("There was an unhandled exception in the host process:", ex);
			};

			using (var host = new NancyHost (new Uri(_hostUri), new BootStrapper(_instanceId), hostSettings)) {
				host.Start ();

				if(_useDiscovery) StandupDiscoveryClient ();

				Logger.InfoFormat (string.Format("ForgetMeNot started, listening on {0}...", _hostUri));

				//wait here, forever!
				_resetEvent.WaitOne ();

				Logger.Info ("ForgetMeNot shutting down...");
				host.Stop ();
				_lease.Unannounce ();
			}
		}

		private static void StandupDiscoveryClient()
		{
			var discoveryServer = OTEnvironmentalConfigManager.AppSettings ["ot.discovery.servers"].Value;
			Logger.DebugFormat ("Announcing to the discovery server [{0}] with host address [{1}]", discoveryServer, _hostUri);
			_discoveryClient = new CSDiscoveryClient ();
			_lease = _discoveryClient.CreateAnnouncement (
				new AnnouncementBuilder()
				.SetServiceType(ServiceName)
				.SetServiceUri(new Uri(discoveryServer)));
		}

		private static bool ParseArgs(string[] args)
		{
			bool show_help = false;	
			var os = new OptionSet ();
			var host = os.AddVariable<string> ("host-name", "The name of the host machine that the service is running on. e.g. localhost or ci-forgetmenot-otenv.com");
			var port = os.AddVariable<int> ("port", "The port to bind to on the host machine");
			var instanceId = os.AddVariable<string> ("instance-id", "A unique identifier for this service instance. Used for clustering and distinguishing between instances of the service");
			var environment = os.AddVariable<string> ("environment", "Optional. The deployment environment. So that ForgetMeNot can load the correct config file. e.g. ci-uswest2");
			var withoutDiscovery = os.AddSwitch ("without-discovery", "Disable discovery announcement");

			if (args.Length == 0) {
				ShowHelp (os);
				return false;
			}

			List<string> extra;
			try {
				extra = os.Parse (args);
				_hostName = host.Value;
				_port = port.Value;
				_instanceId = instanceId.Value;
				_environment = environment.Value;
				_useDiscovery = !withoutDiscovery.Enabled;
			}
			catch (OptionException e) {
				Console.Write ("Forget-Me-Not: ");
				Console.WriteLine (e.Message);
				Console.WriteLine ("Try `--help' for more information.");
				return false;
			}

			if (extra.Count > 0) {
				ShowHelp (os);
				return false;
			}

			if (show_help) {
				ShowHelp (os);
				return false;
			}

			return true;
		}

		static void ShowHelp (OptionSet p)
		{
			Console.WriteLine ("Usage: ForgetMeNot OPTIONS");
			Console.WriteLine ();
			Console.WriteLine ("Options:");
			p.WriteOptionDescriptions (Console.Out);
		}
	}
}
