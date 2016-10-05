using System;
using System.Xml;

namespace SAF.Configuration
{
	/// <summary>
	/// EventNotificationConfiguration is used to retrieve configuration
	/// information for SAF.EventNotification component
	/// </summary>
	public class EventNotificationConfiguration
	{
		private XmlNode enXml;
		public EventNotificationConfiguration(XmlNode configData)
		{
			enXml = configData;
		}
		public string GetEventServerUrl()
		{
			
			string server = GetServer();
			string port =GetPortNumber();
			string appName =GetApplicationName();
			string objectUri =GetObjectUri();
			return "http://" + server + ":" + port + "/" + appName + "/" + objectUri;
		}

		public string GetPortNumber()
		{
			return enXml.SelectSingleNode("Port").InnerText;
		}

		public string GetApplicationName()
		{
			return enXml.SelectSingleNode("ApplicationName").InnerText;
		}

		public string GetObjectUri()
		{
			return enXml.SelectSingleNode("ObjectUri").InnerText;
		}

		public string GetServer()
		{
			return enXml.SelectSingleNode("Server").InnerText;
		}
	}
}
