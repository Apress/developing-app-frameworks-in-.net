using System;
using System.Xml;

namespace SAF.Configuration
{
	/// <summary>
	/// Summary description for ClassFactoryConfiguration.
	/// </summary>
	public class ClassFactoryConfiguration
	{
		private XmlNode classFactoryXml;
		/// <summary>
		/// the constructor is called by the configuraiton manager
		/// </summary>
		/// <param name="configData">the xml element containing the class factory related data</param>
		public ClassFactoryConfiguration(XmlNode configData)
		{
			classFactoryXml = configData;
		}

		/// <summary>
		/// retrieve information about a class stored in the SAF.ClassFactory section
		/// </summary>
		/// <param name="name">name to identity the class factory</param>
		/// <returns></returns>
		public XmlNode GetFactoryData(string name)
		{
			return classFactoryXml.SelectSingleNode("Class[@name='" + name + "']");
		}
	}
}
