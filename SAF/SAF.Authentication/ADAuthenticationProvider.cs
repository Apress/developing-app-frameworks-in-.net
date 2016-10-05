using System;
using System.Collections;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.Xml;
using SAF.Configuration;
using System.Configuration;
using SAF.Utility;
using System.Security.Principal;

namespace SAF.Authentication
{
	/// <summary>
	/// ADIdentity class provide a sample implementation for
	/// Identity object that replies on active directory for its authentication
	/// </summary>

	public class ADIdentity : SAFIdentity
	{
		/// <summary>
		/// ADIdentity public constructor provides user ability to authenticate the user
		/// against the underlying security theme.
		/// </summary>
		/// <param name="userid">user id</param>
		/// <param name="password">password</param>
		/// <param name="domain">active directory domain or local machine name</param>
		/// <param name="applicationName">the name of the application</param>
		public ADIdentity(string userid, string password, string domain, string applicationName)
		{
			//check if the userid and password are valid.
			SecurityUtility su = new SecurityUtility();
			su.ValidateUser(userid,password,domain);
			//set the application name and userid of the identity object
			this.applicationName = applicationName;
			this.name = userid;
		}
		/// <summary>
		/// provide a back door to instantiate the identity object without user id and password
		/// it is used by the ADPrincipal to create ADIdentity object when it doesn't have 
		/// user login info.  The modifier "internal" hide this method when accessed from 
		/// another assembly, such as when developer is creating the ADIdentity object from his 
		/// application code.
		/// </summary>
		/// <param name="userid">user id</param>
		/// <param name="applicationName">application name</param>
//		internal ADIdentity(string userid,string applicationName)
//		{
//			this.ApplicationName = applicationName;
//			this.Name = userid;
//		}
	}

	/// <summary>
	/// ADPrincipal is an IPrincipal object that uses Active directory as it
	/// membership information store.
	/// </summary>
	public class ADPrincipal : IPrincipal
	{
		private IIdentity identity;
		private string searchQuery;
		/// <summary>
		/// IPrincipal interface method
		/// </summary>
		public IIdentity Identity
		{
			get {return identity;}
		}

		/// <summary>
		/// this method is called by SAF.Authentication is create an principal object
		/// for a given user and application name
		/// </summary>
		/// <param name="userid">user id</param>
		/// <param name="applicationName">application name</param>
		public ADPrincipal(SAFIdentity safIdentity)
		{
			identity = safIdentity;
			ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			XmlNode providerData =cm.AuthenticationConfig.GetProviderConfigurationData(safIdentity.ApplicationName);
			//retrieve ADSI query string used to search for the given user.
			searchQuery = providerData.SelectSingleNode("ADDirectoryEntry").InnerText;
		}

		/// <summary>
		/// IPrincipal interface method, it checks the memberhsip information
		/// by calling the AD through System.DirectoryService
		/// </summary>
		/// <param name="role">role name</param>
		/// <returns>true or false to indicate whether a given principle belongs to a given role</returns>
		public bool IsInRole(string role)
		{
			StringCollection groupCollection = new StringCollection();
			DirectoryEntry obEntry = new DirectoryEntry(searchQuery);	
			//search  on the user name. if user doesn't exist, an exception will be thrown
			DirectoryEntry obUser = obEntry.Children.Find(identity.Name,"user");
			
			//retrieve the group information for the given user
			object groups = obUser.Invoke("Groups");
			//loop through each group object to retrieve the group name
			foreach (object o in (IEnumerable)groups)
			{
				DirectoryEntry group= new DirectoryEntry(o);
				groupCollection.Add(group.Name);
			}

			//check if the role is part of the groups the user belongs to.
			return groupCollection.Contains(role);
		}
	}

}
