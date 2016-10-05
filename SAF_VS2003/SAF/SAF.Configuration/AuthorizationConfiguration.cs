using System;
using System.Xml;
using System.Collections;


namespace SAF.Configuration
{
	/// <summary>
	/// AuthorizationConfiguration provides information used by SAF.Authorization component
	/// </summary>
	public class AuthorizationConfiguration
	{
		private static Hashtable providerTable = new Hashtable();
		public XmlNode AuthorizationXml;
		internal AuthorizationConfiguration(XmlNode configData)
		{
			AuthorizationXml = configData;

		}

		/// <summary>
		/// Retrieve roles/membership information for a given user
		/// </summary>
		/// <param name="name">user name</param>
		/// <returns>string array containing user's role information</returns>
		public string[] AllowedRoles(string name)
		{
			XmlNode node = AuthorizationXml.SelectSingleNode("//Permissions/Allows/Allow[@name='"+ name + "']");
			if (node != null)
			{
				string[] roles = node.Attributes["roles"].Value.Split(',');
				return roles;
			}
			return null;
		}

		/// <summary>
		/// Retrieve specifically denied role/membership information 
		/// for a given user.
		/// </summary>
		/// <param name="name">user name</param>
		/// <returns>string array containing user's denied role infomration</returns>
		public string[] DeniedRoles(string name)
		{
			XmlNode node = AuthorizationXml.SelectSingleNode("//Permissions/Denies/Deny[@name='"+ name + "']");
			if (node != null)
			{
				string[] roles = node.Attributes["roles"].Value.Split(',');
				return roles;
			}
			return null;
		}

		/// <summary>
		/// Retrieve an object that is used to authorize 
		/// the access for a given user.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public object GetAuthorizationProvider (string name)
		{
			XmlNode node = AuthorizationXml.SelectSingleNode("//Permissions/Allows/Allow[@name='"+ name + "']");
			string provider = null;
			if (node != null)
			{
				//retrieve the provider from cache.
				provider = node.Attributes["provider"].Value;
			}
			else
			{
				node = AuthorizationXml.SelectSingleNode("//Permissions/Denies/Deny[@name='"+ name + "']");
				provider = node.Attributes["provider"].Value;
			}
			if (providerTable[provider] == null)
			{
				//retrieve the type information of the provider
				XmlNode providerNode =AuthorizationXml.SelectSingleNode("//Providers/Provider[@name='"+ provider + "']");
				string typeInfo = providerNode.Attributes["type"].Value;
				Type type = Type.GetType(typeInfo);
				//create the provider object
				object providerObject = Activator.CreateInstance(type,null);
				//cache the provider object into the internal hashtable.
				providerTable.Add(provider,providerObject); 
			}
			//Retrieve the cached proivder from cache.
			object ap = providerTable[provider];
			return ap;
		
		}
	}
}
