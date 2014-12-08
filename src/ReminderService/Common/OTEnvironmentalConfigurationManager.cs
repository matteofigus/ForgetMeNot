using System;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;

namespace ReminderService.Common
{
	public static class OTEnvironmentalConfigManager
	{
		const string CI_ENV 				= "ci-uswest2";
		const string PP_ENV 				= "pp-uswest2";
		const string PROD_ENV 				= "prod-uswest2";
		const string ConfigRoot 			= "src/ReminderService/ReminderService.Hosting.NancySelf/bin/Debug/config/";
		//const string ConfigRoot 			= "src/config/";

		private static Configuration _config;

		public static string Environment {
			get;
			set;
		}

		public static KeyValueConfigurationCollection AppSettings {
			get {
				CheckAndLoadConfig ();
				return _config.AppSettings.Settings; 
			}
		}

		public static ConnectionStringSettingsCollection ConnectionStrings {
			get { 
				CheckAndLoadConfig ();
				return _config.ConnectionStrings.ConnectionStrings; 
			}
		}

		public static object GetSection (string sectionName)
		{
			CheckAndLoadConfig ();
			return _config.GetSection (sectionName);
		}

		private static void CheckAndLoadConfig()
		{
			if (string.IsNullOrEmpty (Environment))
				return;

			if (_config == null)
				LoadConfigForEnvironment (Environment);
		}

		private static Configuration LoadConfigForEnvironment(string environment)
		{
			string configPath = string.Empty;

			switch (environment) {
			case (CI_ENV):
				configPath = ConfigRoot + CI_ENV + "/app.config"; 
				Console.WriteLine ("OT_ENV set to 'ci'; loading the ci config file");
				break;
			case (PP_ENV):
				configPath = ConfigRoot + PP_ENV + "/app.config"; 
				Console.WriteLine ("OT_ENV set to 'pp'; loading the preprod config file");
				break;
			case (PROD_ENV):
				configPath = ConfigRoot + PROD_ENV + "/app.config"; 
				Console.WriteLine ("OT_ENV set to 'prod'; loading the prod config file");
				break;
			default:
				Console.WriteLine ("OT_ENV set to 'dev' or not set at all; using the default config file");
				configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
				break;
			}

			ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
			var path = Path.Combine(System.Environment.CurrentDirectory, configPath);
			Console.WriteLine ("Loading configuration file from: " + path);
			configMap.ExeConfigFilename = path;
			return ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
		}
	}
}



