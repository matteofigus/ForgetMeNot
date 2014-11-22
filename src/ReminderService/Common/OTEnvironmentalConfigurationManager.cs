using System;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;

namespace ReminderService.Common
{
	public static class OTEnvironmentalConfigManager
	{
		const string Deploy_Environment 	= "OT_ENV";
		const string Default_Environment 	= "dev";
		const string CI_ENV 				= "ci";
		const string PP_ENV 				= "pp";
		const string PROD_ENV 				= "prod";
		const string ConfigRoot 			= "src/ReminderService/ReminderService.Hosting.NancySelf/bin/Debug/config/";

		private static Configuration _config;

		static OTEnvironmentalConfigManager ()
		{
			var env = GetEnvironment ();
			_config = LoadConfigForEnvironment (env);
		}

		public static KeyValueConfigurationCollection AppSettings {
			get { return _config.AppSettings.Settings; }
		}

		public static ConnectionStringSettingsCollection ConnectionStrings {
			get { return _config.ConnectionStrings.ConnectionStrings; }
		}

		public static object GetSection (string sectionName)
		{
			return _config.GetSection (sectionName);
		}

		private static string GetEnvironment ()
		{
			var env = Environment.GetEnvironmentVariable (Deploy_Environment);
			if (string.IsNullOrEmpty (env))
				env = Default_Environment;

			return env;
		}

		private static Configuration LoadConfigForEnvironment(string environment)
		{
			string configPath = string.Empty;

			switch (environment) {
			case (CI_ENV):
				configPath = ConfigRoot + "ci/app.config"; 
				Console.WriteLine ("OT_ENV set to 'ci'; loading the ci config file");
				break;
			case (PP_ENV):
				configPath = ConfigRoot + "pp/app.config"; 
				Console.WriteLine ("OT_ENV set to 'pp'; loading the preprod config file");
				break;
			case (PROD_ENV):
				configPath = ConfigRoot + "prod/app.config"; 
				Console.WriteLine ("OT_ENV set to 'prod'; loading the prod config file");
				break;
			default:
				Console.WriteLine ("OT_ENV set to 'dev' or not set at all; using the default config file");
				configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
				break;
			}

			ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
			var path = Path.Combine(Environment.CurrentDirectory, configPath);
			Console.WriteLine ("Loading configuration file from: " + path);
			configMap.ExeConfigFilename = path;
			return ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
		}
	}
}



