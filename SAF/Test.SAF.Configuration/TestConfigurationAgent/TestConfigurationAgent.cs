using System;
using SAF.Configuration;
using System.Xml;

namespace TestConfigurationAgent
{
	/// <summary>
	/// A sample Agent class that is responsible for retrieve configuration
	/// data stored in other system via web service.
	/// </summary>
	public class ConfigurationWSAgent :  IConfigurationAgent
	{
		private string section;
		private string environment;
		private string url;
		public ConfigurationWSAgent(){}
		/// <summary>
		/// this method sets up the agent with parameters information 
		/// defined in the configuration file.
		/// </summary>
		/// <param name="configData"></param>
		public void Initialize(XmlNode configData)
		{
			section= configData.SelectSingleNode("Parameters/Section").InnerText;
			environment = configData.SelectSingleNode("Parameters/Environment").InnerText;

			url = configData.SelectSingleNode("Url").InnerText;
		}

		/// <summary>
		/// this method call the web service and retrieve the actual configration data
		/// </summary>
		/// <returns>the configuration data</returns>
		public string GetConfigurationSetting()
		{
			localhost.ConfigurationService cs = new localhost.ConfigurationService();
			cs.Url = url;
			return cs.GetConfiguration(section,environment);
		}
			
	}
}
