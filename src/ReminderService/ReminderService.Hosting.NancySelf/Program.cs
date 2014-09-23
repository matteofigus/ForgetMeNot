using System;
using Nancy.Hosting.Self;
using OpenTable.Services.Components.DiscoveryClient;
using ReminderService.API.HTTP.BootStrap;
using log4net;
using log4net.Config;

namespace ReminderService.Hosting.NancySelf
{
	class MainClass
	{
		const string ServiceName = "forgetmenot";
		private static string _hostUri = "http://localhost:8080";
		private static ILog Logger = LogManager.GetLogger("ForgetMeNot.SelfHosted.Host");
		private static CSDiscoveryClient _discoveryClient;
		private static IAnnouncementLease _lease;

		public static void Main (string[] args)
		{
			XmlConfigurator.Configure ();

			Logger.Info ("Starting ForgetMeNot service...");

			using (var host = new NancyHost (new Uri(_hostUri), new BootStrapper())) {
				host.Start ();

				StandupDiscoveryClient ();

				Logger.InfoFormat (string.Format("ForgetMeNot started, listening on {0}...", _hostUri));
				Console.ReadLine ();
				_lease.Unannounce ();
			}
		}

		private static void StandupDiscoveryClient()
		{
			_discoveryClient = new CSDiscoveryClient ();
			_lease = _discoveryClient.CreateAnnouncement (
				new AnnouncementBuilder()
				.SetServiceType(ServiceName)
				.SetServiceUri(new Uri(_hostUri)));
		}
	}
}
