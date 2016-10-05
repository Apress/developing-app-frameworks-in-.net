using System;
using System.Xml;

namespace SAF.Configuration
{
	/// <summary>
	/// ServiceConfiguration retrieve configuration information
	/// for SAF.WindowService component.
	/// </summary>
	public class ServiceConfiguration
	{
		public XmlNode ServicesXml;
		internal ServiceConfiguration(XmlNode configData)
		{
			if (configData != null)
			{
				ServicesXml = configData;
			}
		}
	}

	
}
