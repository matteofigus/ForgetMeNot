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

namespace ReminderService.Hosting.NancySelf
{
	class MainClass
	{
		const string ServiceName = "forgetmenot";
		private static string _hostUri;// = "http://localhost:8080"; //TODO: make this a system call to discover the host name
		private static ILog Logger = LogManager.GetLogger("ForgetMeNot.SelfHosted.Host");
		private static CSDiscoveryClient _discoveryClient;
		private static IAnnouncementLease _lease;
		private static bool _useDiscovery = true;
		private static ManualResetEvent _resetEvent = new ManualResetEvent (false);

		public static void Main (string[] args)
		{
			XmlConfigurator.Configure ();

			Logger.Info ("Starting ForgetMeNot service...");

			if (args.Length != 0) {
				if (args [0] == "--without-discovery")
					_useDiscovery = false;
			}

			_hostUri = OTEnvironmentalConfigManager.AppSettings ["host-uri"].Value;
				var hostSettings = new HostConfiguration ();
			hostSettings.UnhandledExceptionCallback = ex => {
				Logger.Error("There was an error encountered in the host process:", ex);
			};

			using (var host = new NancyHost (new Uri(_hostUri), new BootStrapper())) {
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
	}
}
