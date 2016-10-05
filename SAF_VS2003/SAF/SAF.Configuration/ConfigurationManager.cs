using System;
using System.Xml;
using System.Reflection;
using System.Runtime.Remoting;

namespace SAF.Configuration
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class ConfigurationHandler  : System.Configuration.IConfigurationSectionHandler
	{
		public object Create(Object parent, object configContext, XmlNode section)
		{
			Type type = System.Type.GetType(section.Attributes["type"].Value);
			object[] parameters = {section};
			//call the configuration object's constructor
			object configObject = null;
			try
			{
				configObject = Activator.CreateInstance(type, parameters);
			}
			catch (Exception ex)
			{
				string x = ex.Message;
				return null;
			}
			return configObject;
		
		}
	}


	/// <summary>
	/// Provides access to configuraiton object for the 
	/// framework component
	/// </summary>
	public class ConfigurationManager
	{
		public SAF.Configuration.ServiceConfiguration ServiceConfig;
		public SAF.Configuration.AuthorizationConfiguration AuthorizationConfig;
		public SAF.Configuration.CryptographyConfiguration CryptographyConfig;
		public SAF.Configuration.AuthenticationConfiguration AuthenticationConfig;
		public SAF.Configuration.ClassFactoryConfiguration ClassFactoryConfig;
		public SAF.Configuration.EventNotificationConfiguration EventNotificationConfig;
		public SAF.Configuration.CacheConfigration CacheConfig;
		public SAF.Configuration.MessageQueueConfiguration MessageQueueConfig;
		private XmlNode configurationData;


		/// <summary>
		/// Initialize all the configuration objects accessible through 
		/// this configuration manager.
		/// </summary>
		/// <param name="sections"></param>
		public  ConfigurationManager (XmlNode sections)
		{
			configurationData = sections;
			ConfigurationAgentManager cam = new ConfigurationAgentManager(configurationData);
			ServiceConfig = new ServiceConfiguration(cam.GetData("SAF.WindowsService"));
			AuthorizationConfig = new AuthorizationConfiguration(cam.GetData("SAF.Authorization"));
			CryptographyConfig = new CryptographyConfiguration(cam.GetData("SAF.Cryptography"));
			AuthenticationConfig = new AuthenticationConfiguration(cam.GetData("SAF.Authentication"));
			ClassFactoryConfig = new ClassFactoryConfiguration(cam.GetData("SAF.ClassFactory"));
			EventNotificationConfig = new EventNotificationConfiguration(cam.GetData("SAF.EventNotification"));
			MessageQueueConfig = new MessageQueueConfiguration(cam.GetData("SAF.MessageQueue"));
			CacheConfig = new  CacheConfigration(cam.GetData("SAF.Cache"));
	
		}
	}
}
