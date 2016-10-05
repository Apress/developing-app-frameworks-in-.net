using System;
using System.Xml;

namespace SAF.Configuration
{
	/// <summary>
	/// Summary description for CacheConfigration.
	/// </summary>
	public class CacheConfigration
	{
		private XmlNode cacheXml;
		public CacheConfigration(XmlNode configData)
		{
			cacheXml = configData;
		}

		/// <summary>
		/// Load the cache strategy object defined in the
		/// configuraiton file and return to the SAF.Cache
		/// </summary>
		/// <returns>cache strategy object</returns>
		public object GetCacheStrategy()
		{
			string typeName = cacheXml.SelectSingleNode("CacheStrategy").Attributes["type"].Value;
			Type type = Type.GetType(typeName);
			return Activator.CreateInstance(type,null);
		}
	}
}
