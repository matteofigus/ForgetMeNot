using System;
using Nancy.Hosting.Self;
using ReminderService.API.HTTP.BootStrap;
using log4net;
using log4net.Config;

namespace ReminderService.Hosting.NancySelf
{
	class MainClass
	{
		private static string _hostUri = "http://localhost:8080";
		private static ILog Logger = LogManager.GetLogger("ForgetMeNot.SelfHosted.Host");

		public static void Main (string[] args)
		{
			XmlConfigurator.Configure ();

			Logger.Info ("Starting ForgetMeNot service...");

			using (var host = new NancyHost (new Uri(_hostUri), new BootStrapper())) {
				host.Start ();
				Logger.InfoFormat (string.Format("ForgetMeNot started, listening on {0}...", _hostUri));
				Console.ReadLine ();
			}
		}
	}
}
