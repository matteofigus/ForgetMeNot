using System;
using System.Configuration;
using System.IO;
using System.Collections.Specialized;
using log4net;

namespace ReminderService.Common
{
	public static class OTEnvironmentalConfigManager
	{
		const string Default_Environment 	= "dev";
		const string ConfigRoot 			= "config/";
		const string LogMessageBase 		= "Environment set to '{0}'; loading the config file";

		private static readonly ILog Logger = LogManager.GetLogger("OTEnvironmentalConfigManager");
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
				return _config.AppSettings.Settings; 
			}
		}

		public static ConnectionStringSettingsCollection ConnectionStrings {
			get { 
				return _config.ConnectionStrings.ConnectionStrings; 
			}
		}

		public static object GetSection (string sectionName)
		{
			return _config.GetSection (sectionName);
		}

		private static void LoadConfig(string environment)
		{
			string pathToConfigFile;

			if (string.IsNullOrEmpty (environment) || environment == Default_Environment) {
				Logger.Info ("Environment set to [dev] or not set at all; using the default config file");
				pathToConfigFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			} else {
				Logger.InfoFormat ("Environment set to [{0}]", environment);
				pathToConfigFile = Path.Combine (ConfigRoot, environment, "app.config");
			}

			var path = Path.Combine(System.Environment.CurrentDirectory, pathToConfigFile);

			if (!File.Exists (path))
				throw new FileNotFoundException (string.Format("Could not find the config file for environment [{0}]. Expected path to be: {1}", environment, path));
				
			ExeConfigurationFileMap configMap = new ExeConfigurationFileMap();
			Logger.InfoFormat ("Loading configuration file from: {0}", path);
			configMap.ExeConfigFilename = path;
			_config =  ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
		}
	}
}



