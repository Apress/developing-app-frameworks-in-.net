using System;
using System.Security;
using System.Security.Permissions;

namespace SAF.Authorization
{
	/// <summary>
	/// SAFSecurityAttribute provide an attribute which can be applies 
	/// to the members of a class to perform access permission on the caller.
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.All, AllowMultiple = true)]
	public class SAFSecurityAttribute : CodeAccessSecurityAttribute
	{
		private string name;
		public SAFSecurityAttribute( SecurityAction action ) : base( SecurityAction.Demand )
		{
			this.Unrestricted = true;
		}

		//A factory method that return an IPermission object which will be used
		//later to perform the access permission.
		public override IPermission CreatePermission()
		{
			//create the SAFSecurityPermission object
			SAFSecurityPermission permission = new SAF.Authorization.SAFSecurityPermission();
			//set its properties
			permission.Name = this.Name;
			permission.Unrestricted = this.Unrestricted;
			return permission;

		}

		/// <summary>
		/// the properties to identity the entity marked with the SAFSecurityAttribute
		/// </summary>
		public string Name
		{
			get{return name;}
			set{name = value;}
		}
	}
}
