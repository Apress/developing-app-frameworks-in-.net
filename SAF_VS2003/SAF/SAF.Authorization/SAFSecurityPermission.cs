using System;
using System.Security;
using System.Security.Permissions;
using SAF.Configuration;
using SAF.Library.Security;
using System.Configuration;
using System.Diagnostics;

namespace SAF.Authorization
{
	/// <summary>
	/// SAFSecurityPermission is responsible for determine the access permission 
	/// of caller.
	/// </summary>
	public class SAFSecurityPermission : IPermission, IUnrestrictedPermission
	{
		public bool Unrestricted ;
		public string Name;
		public SAFSecurityPermission()
		{
		}
		public SAFSecurityPermission(PermissionState state )
		{
		}

		/// <summary>
		/// Demand method is called at runtime when a caller is trying to access
		/// resource that is marked with SAFSecurityAttribute.
		/// </summary>
		public void Demand()
		{
			//obtain the information about the denied and allowed roles 
			//information from configuraiton file
			
			ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			AuthorizationConfiguration ac = cm.AuthorizationConfig;
			string[] allowedRoles = ac.AllowedRoles(Name);
			string[] deniedRoles = ac.DeniedRoles(Name);

			//create the IAuthorizationProvider object which is responsible
			//for verify if the call is permitted or denied based on the 
			//allowed role and denied role information.
			IAuthorizationProvider ap =(IAuthorizationProvider) ac.GetAuthorizationProvider(Name);
			ap.Authorize(allowedRoles, deniedRoles);	
		}

		public bool IsUnrestricted()
		{
			return Unrestricted;
		}

		/// <summary>
		/// return an instance of underlying SecurityPermission object
		/// </summary>
		/// <returns></returns>
		public IPermission Copy()
		{
			SAFSecurityPermission copy = new SAFSecurityPermission();
			copy.Unrestricted = Unrestricted;
			return copy;
		}
		/// <summary>
		/// FromXml is called by the CLR to set the properties of 
		/// IPermission object at runtime
		/// </summary>
		/// <param name="securityElement">the xml contain the SAFSecurityAttribute information</param>
		public void FromXml(SecurityElement securityElement)
		{
			//retrieve the information from SecurityElement, an special Xml document
			//and set the object state (fields/properties) with the values from 
			//the SecurityElement.
			string element = securityElement.Attribute("Unrestricted");         
			Name = securityElement.Attribute("Name");
			if(null != element)
			{  
				this.Unrestricted = Convert.ToBoolean(element);
			}
			
		}

		/// <summary>
		/// ToXml is called by compiler during compilation.  It produces
		/// a SecurityElement object which contains the SAFSecurityAttribute inforamtion
		/// The information will aslo be stored to the assembly's metadata by the 
		/// compiler.
		/// </summary>
		/// <returns>SecurityElement object contains the SAFSecurityAttribute information.</returns>
		public SecurityElement ToXml()
		{ 
			SecurityElement securityElement = new SecurityElement("Permission");
			Type type = this.GetType();
			//add the attribute information to the SecurityElement object. doing reverse of FromXml method.
			securityElement.AddAttribute("class", type.AssemblyQualifiedName);
			securityElement.AddAttribute("Unrestricted", Unrestricted.ToString());
			securityElement.AddAttribute("Name",Name);
			return securityElement;
		}
		public bool IsSubsetOf(IPermission target)
		{
			return false;
		}
		public IPermission Union( IPermission target )
		{
			return null;
		}
		
		public IPermission Intersect(IPermission target)
		{
			return null;
		}
	}

}
