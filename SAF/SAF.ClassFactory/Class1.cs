using System;
using System.Xml;
using System.Configuration;
using SAF.Configuration;

namespace SAF.ClassFactory
{
	/// <summary>
	/// class factory service, used to obtain the abstract factory class.
	/// </summary>
	public class ClassFactory
	{
		private ClassFactory()
		{
		}

		/// <summary>
		/// Called by the client to get an instance of the factory class
		/// </summary>
		/// <param name="factoryName">factory name</param>
		/// <returns>class factory object</returns>
		public static object GetFactory(string factoryName)
		{
			object factory = null;
			ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			ClassFactoryConfiguration cf=  cm.ClassFactoryConfig;
			XmlNode classFactoryData = cf.GetFactoryData(factoryName);		
	
			//obtain the type information
			string type = classFactoryData.Attributes["type"].Value;
			Type t = System.Type.GetType(type);
			//creat an instance of concrete class factory
			if (classFactoryData.Attributes["location"] != null)
			{
				string location = classFactoryData.Attributes["location"].Value;
				factory = Activator.GetObject(t,location);
			}
			else
			{
				factory = Activator.CreateInstance(t,null);
			} 
			return factory;
		}

	}
}
