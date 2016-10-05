using System;
using System.Security.Principal;
using System.Configuration;
using SAF.Configuration;

namespace SAF.Authentication
{
	/// <summary>
	/// SAFPrincipal encapsulate identity and membership information
	/// for a caller on specific application
	/// </summary>
	public class SAFPrincipal : IPrincipal
	{
		private IIdentity identity;
		//internal application specific principal object
		private IPrincipal currentApplicationPrincipal;
		private string safUser;

		/// <summary>
		/// constructor takes the SAFIdentity as its parameter.
		/// </summary>
		/// <param name="sid">SAFIdentity object which contain the identity and appplication name information</param>
		public SAFPrincipal(SAFIdentity sid)
		{
			//retrieve the authentication configuraiton from the configuraiton file
			ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			AuthenticationConfiguration ac = cm.AuthenticationConfig;

			//set identity object and the SAFUser property
			identity = sid;
			safUser = ac.GetSAFUserName(sid.Name,sid.ApplicationName);
			//set the application inforamtion of the SAFPrincipal
			SetApplication(sid.ApplicationName);
		
		}

		/// <summary>
		/// IPrincipal interface method that determines
		/// whether the current pricipal is member of a given role
		/// </summary>
		/// <param name="role">the role name</param>
		/// <returns>true or false on whether the principal is member of the role</returns>
		public bool IsInRole(string role)
		{
			return currentApplicationPrincipal.IsInRole(role);
		}
		public IIdentity Identity
		{
			get {return identity;}
		}
		public string SAFUser
		{
			get {return safUser;}
		}

		/// <summary>
		/// SetApplication will change the application context of principal object
		/// so that principal object represents the user membership information of
		/// a given application
		/// </summary>
		/// <param name="application">name of the application for which SAFPrincipal will switch the its membership information.</param>
		public void SetApplication(string application)
		{
			ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			AuthenticationConfiguration ac = cm.AuthenticationConfig;
			//retrieve the type informatioin for the principal object of a given applicaiton
			string typeName = ac.GetPrincipalTypeForApplication(application);
			string appUserName = cm.AuthenticationConfig.GetIdentityForApplicaiton(safUser,application);
			SAFIdentity safIdentity = new SAFIdentity(appUserName,application);
			Type type = Type.GetType(typeName);
			object[] parameters = new object[1]{safIdentity};
			//set the new object to the internal principal object.
			currentApplicationPrincipal = (IPrincipal)Activator.CreateInstance(type,parameters);
			identity = (IIdentity)safIdentity;
		}
	}
}
