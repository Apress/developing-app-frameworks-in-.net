using System;
using System.Xml;

namespace SAF.Application.Configuration
{
	/// <summary>
	/// Application specific configuration manager. its design follows
	/// that of ConfigurationManager class in SAF.Configuration
	/// </summary>
	public class ConfigurationManager
	{
		public DocumentLayerConfiguration DocumentLayerConfig;
		public WorkFlowConfiguration WorkFlowConfig;

		/// <summary>
		/// constructor that takes the xml configuration info and
		/// initialize the configuration object to which it holds refereces.
		/// </summary>
		/// <param name="sections">XmlNode that hold the configuration information</param>
		public ConfigurationManager(XmlNode sections)
		{
			//initialize configuration objects
			DocumentLayerConfig  = new DocumentLayerConfiguration(sections.SelectSingleNode("DocumentLayers"));
			WorkFlowConfig = new WorkFlowConfiguration(sections.SelectSingleNode("WorkFlow"));

		}
	}
}
