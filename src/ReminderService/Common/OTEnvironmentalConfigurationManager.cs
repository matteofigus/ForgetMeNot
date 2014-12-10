using System;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;

namespace ReminderService.Common
{
	public static class OTEnvironmentalConfigManager
	{
		//const string Deploy_Environment 	= "OT_ENV";
		const string Default_Environment 	= "dev";
		//const string CI_ENV 				= "ci";
		//const string PP_ENV 				= "pp";
		//const string PROD_ENV 				= "prod";
		const string ConfigRoot 			= "config/";
		const string LogMessageBase 		= "Environment set to '{0}'; loading the config file";

		//private static readonly ILog Logger = LogManager.GetLogger();
		private static Configuration _config;
		private static string _environment;

		public static void SetEnvironment(string environment)
		{
			_environment = environment;
			LoadConfig (environment);
		}

		public static string Environment 
		{
			get { return _environment; }
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

		private static void LoadConfig(string environment)
		{
			string pathToConfig;
			string pathToConfigFile;

			if (string.IsNullOrEmpty (environment) || environment == Default_Environment) {
				Console.WriteLine ("OT_ENV set to 'dev' or not set at all; using the default config file");
				pathToConfigFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			} else {
				pathToConfig = Path.Combine (ConfigRoot, environment);
				pathToConfigFile = Path.Combine (pathToConfig, "app.config");
			}

			if (!File.Exists (pathToConfigFile))
				throw new FileNotFoundException (string.Format("The config file for environment [{0}] does not exist.", environment));
				
			ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
			var path = Path.Combine(System.Environment.CurrentDirectory, pathToConfigFile);
			Console.WriteLine ("Loading configuration file from: " + path);
			configMap.ExeConfigFilename = path;
			_config =  ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
		}



//		private static Configuration LoadConfigForEnvironment(string environment)
//		{
//			string configPath = string.Empty;
//
//			switch (environment) {
//			case (CI_ENV):
//				configPath = Path.Combine(_pathToConfig, "app.config"); 
//				Console.WriteLine (string.Format(LogMessageBase, environment));
//				break;
//			case (PP_ENV):
//				configPath = ConfigRoot + "pp/app.config"; 
//				Console.WriteLine (string.Format(LogMessageBase, environment));
//				break;
//			case (PROD_ENV):
//				configPath = ConfigRoot + "prod/app.config"; 
//				Console.WriteLine (string.Format(LogMessageBase, environment));
//				break;
//			default:
//				Console.WriteLine ("OT_ENV set to 'dev' or not set at all; using the default config file");
//				configPath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
//				break;
//			}
//
//
//		}
	}
}



