using System;
using System.Xml;
using System.Collections;


namespace SAF.Configuration
{
	/// <summary>
	/// It is responsible for loading the agent object which
	/// is responsible for retrieving the configuration data
	/// </summary>
	/// 
	public class ConfigurationAgentManager
	{
		private XmlNode configurationData;
		public ConfigurationAgentManager(XmlNode configData)
		{
			configurationData = configData;
		}
		/// <summary>
		/// it return the Xml containing the configuraiton settings for a given key 
		/// </summary>
		/// <param name="key">name of the Xml section in the configuration file, such as <SAF.ClassFactory>. </param>
		/// <returns>XmlNode that contains the configuration settings </returns>
		public XmlNode GetData(string key)
		{
			XmlNode result=null;
			XmlAttribute agentAttribute =null;
			if (configurationData.SelectSingleNode(key) != null)
			{
				//check if there is agent defined for a particular section or key
				//if there is, load the agent and make it retrieve the data
				//otherwise, just load the data from the configuraiton file
				agentAttribute = configurationData.SelectSingleNode(key).Attributes["ConfigurationAgent"];
				if ( agentAttribute == null)
				{
					result = configurationData.SelectSingleNode(key);
				}
				else
				{
					//retrive the data using the agent
					string data = GetAgent(agentAttribute.Value).GetConfigurationSetting();
					XmlDocument xml = new XmlDocument();
					xml.LoadXml(data);
					result = (XmlNode)xml.DocumentElement;
				}
			}
			return result;
		}

		/// <summary>
		/// the method load the agent using reflection and return an instance of agent 
		/// to the caller
		/// </summary>
		/// <param name="agentName">name of the agent referenced in the configuration file</param>
		/// <returns>an agent object</returns>
		private IConfigurationAgent GetAgent(string agentName)
		{
			XmlNode agentNode = configurationData.SelectSingleNode("//Agent[@name ='" + agentName +  "']");
			Type type = Type.GetType(agentNode.Attributes["type"].Value);
			IConfigurationAgent agent = (IConfigurationAgent)Activator.CreateInstance(type,null);
			//Initialize method setup the agent object with the parameter information specified
			//in the file that is needed for the agent to do its job
			agent.Initialize(agentNode);
			return agent;
		}
	}
	/// <summary>
	/// Interface that each agent class must implement.
	/// its two methods are called by agent manager at runtime.
	/// </summary>
	public interface IConfigurationAgent
	{
		void Initialize(XmlNode xml);
		string GetConfigurationSetting();
	}
}
