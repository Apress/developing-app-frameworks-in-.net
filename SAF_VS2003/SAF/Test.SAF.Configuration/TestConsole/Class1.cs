using System;
using System.Configuration;
using Application.Configuration;
using SAF.Configuration;

namespace TestConsole
{
	/// <summary>
	/// The demo shows how to use SAF.Configuration service inside a program and
	/// how to use the custom configuration object defined for the business application
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//get the configuraiton manager of Application.Configuration 
			AppConfigurationManager cm1 = (AppConfigurationManager)ConfigurationSettings.GetConfig("MyApplication");
			//retrieve the configuraiton data by calling the properties and methods of configuration manager.
			string connection = cm1.DatabaseConfig.GetDatabaseConnection();
			//these two calls involve the agents which retrieve the configuraiton data via web service
			//at http://localhost/ConfigurationData/ConfigurationService.asmx
			string customerQueueLocation = cm1.MessageQueueConfig.GetCustomerQueueLocation();
			string accountQueueLocation = cm1.MessageQueueConfig.GetAccountQueueLocation();
			
			//get the configuration manager of SAF.Configuration
			ConfigurationManager  cm2 = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			string eventServerUrl = cm2.EventNotificationConfig.GetEventServerUrl();

		}
	}
}
