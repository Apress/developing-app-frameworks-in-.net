using System;
using System.Xml;
using SAF.Configuration;

namespace Application.Configuration
{
	/// <summary>
	/// A sample configuration system for the business application
	/// it shows how you can extend the SAF.Configuraiton with your custom
	/// configuration manager and objectsl
	/// </summary>
	public class AppConfigurationManager
	{
		public DatabaseConfiguration DatabaseConfig;
		public MessageQueueConfiguration MessageQueueConfig;
		private XmlNode configurationData;
		/// <summary>
		/// the constructor start the chain reaction of 
		/// creating all the configuration object used by the application
		/// </summary>
		/// <param name="sections">the xml element containing the configuraiton settings</param>
		public AppConfigurationManager(XmlNode sections)
		{
			configurationData = sections;
			ConfigurationAgentManager cam = new ConfigurationAgentManager(configurationData);
			//create the indivdual configuraiton object and assign them to the public fields
			DatabaseConfig = new DatabaseConfiguration(cam.GetData("Application.Database"));
			MessageQueueConfig = new MessageQueueConfiguration(cam.GetData("Application.MessageQueue"));
		}
	}

	/// <summary>
	/// Sample configuraiton object
	/// </summary>
	public class DatabaseConfiguration 
	{
		
		private XmlNode databaseXml;
		/// <summary>
		/// constructor is called by the configuration manager.
		/// </summary>
		/// <param name="configData"></param>
		public DatabaseConfiguration (XmlNode configData)
		{
			databaseXml = configData;	
		}
		/// <summary>
		/// method that parse the information out of the Xml
		/// </summary>
		/// <returns></returns>
		public string GetDatabaseConnection()
		{
			return databaseXml.SelectSingleNode("ConnectionString").InnerText;
		}
	}

	/// <summary>
	/// sample configuration object
	/// </summary>
	public class MessageQueueConfiguration
	{
		private XmlNode mqXml;
		/// <summary>
		/// the constructor is called by configuration manager
		/// </summary>
		/// <param name="configData"></param>
		public MessageQueueConfiguration(XmlNode configData)
		{
			mqXml = configData;
		}

		/// <summary>
		/// method that parse the information out of the Xml
		/// </summary>
		/// <returns></returns>
		public string GetCustomerQueueLocation()
		{
			return mqXml.SelectSingleNode("CustomerQueue").InnerText;
		}

		/// <summary>
		/// method that parse the information out of the Xml
		/// </summary>
		/// <returns></returns>
		public string GetAccountQueueLocation()
		{
			return mqXml.SelectSingleNode("AccountQueue").InnerText;
		}
	}
}
