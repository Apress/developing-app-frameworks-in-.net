using System;
using System.Security.Principal;
using System.Xml;


namespace SAF.Configuration
{
	/// <summary>
	/// AuthenticationConfiguration provides
	/// access to various configuration information used by
	/// SAF.Authentication component.
	/// </summary>
	public class AuthenticationConfiguration
	{
		private XmlNode authXml;
		internal AuthenticationConfiguration(XmlNode configData)
		{
			authXml = configData;
		}

		/// <summary>
		/// Retrieve the type information of the principal used by a given application
		/// </summary>
		/// <param name="appName">application name</param>
		/// <returns></returns>
		public string GetPrincipalTypeForApplication(string appName)
		{
			XmlNode appNode = authXml.SelectSingleNode("//Applications/Application[@name='" + appName + "']");
			string principalType = appNode.Attributes["principal_type"].Value;
			return principalType;
		}

		/// <summary>
		/// Retrieve the identity information with a given user name and applicaiton name
		/// </summary>
		/// <param name="userName">user name</param>
		/// <param name="appName">application name</param>
		/// <returns></returns>
		public string GetIdentityForApplicaiton (string userName, string appName)
		{
			XmlNode identityNode = authXml.SelectSingleNode("//Identities/Identity[@name='" + userName + "']");
			string newUserName = identityNode.SelectSingleNode("Application[@name='" + appName + "']").Attributes["id"].Value;
			return newUserName;
		}

		/// <summary>
		/// retrieve the SAF user name with a given application specific user name and
		/// the applicaiton name.
		/// </summary>
		/// <param name="appUserName">application specific user name</param>
		/// <param name="appName">application name</param>
		/// <returns>SAF user name</returns>
		public string GetSAFUserName(string appUserName, string appName)
		{
			XmlNode safIdentity = authXml.SelectSingleNode("//Identities/Identity/Application[@name='" + appName + "' and @id='" + appUserName + "']");
			string safUserName = safIdentity.ParentNode.Attributes["name"].Value;
			return safUserName;
		}

		/// <summary>
		/// retrieve the information on SAFIdentity and SAFPrincipal for a given application
		/// </summary>
		/// <param name="appName">application name</param>
		/// <returns></returns>
		public XmlNode GetProviderConfigurationData(string appName)
		{
			return authXml.SelectSingleNode("//Applications/Application[@name='" + appName + "']");

		}
	}
}
