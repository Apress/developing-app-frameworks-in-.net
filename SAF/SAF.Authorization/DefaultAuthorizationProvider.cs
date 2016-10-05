using System;
using SAF.Library.Security;
using System.Security.Principal;
using System.Threading;

namespace SAF.Authorization
{
	/// <summary>
	/// A sampel authorization provider that shows how to create one to
	/// be used with SAF.Authorization.
	/// </summary>
	public class GenericAuthorizationProvider : IAuthorizationProvider
	{
		//default constructor
		public GenericAuthorizationProvider()
		{
		}

		/// <summary>
		/// It is called by the SAFSecurityPermission.  It performs the
		/// membership check on caller's current principal (principal must be
		/// GenericPrincipal)
		/// </summary>
		/// <param name="allowedRoles">string array containing the roles allowed</param>
		/// <param name="deniedRoles">string array containing the roles denied</param>
		public void Authorize(string[] allowedRoles, string[] deniedRoles)
		{
			//ensure the current principal is GenenricPrinciple
			if (Thread.CurrentPrincipal.GetType() != typeof(GenericPrincipal))
			{
				throw new SystemException("Current requestion doesn't have Generic principal");
			}
			bool allowed = false;
			//because deny permission override the allow permission, try checking the deny roles first.
			if (deniedRoles != null)
			{
				foreach (string role in deniedRoles)
				{
					//if caller is member of denied role, reject the call by throwing exception
					if (Thread.CurrentPrincipal.IsInRole(role))
					{
						throw new System.Security.SecurityException("Access denied to account " + Thread.CurrentPrincipal.Identity.Name);
					}
				}
			}

			//check if the caller is member of allowed roles.
			if (allowedRoles !=null)
			{
				foreach (string role in allowedRoles)
				{
					if (Thread.CurrentPrincipal.IsInRole(role))
					{
						allowed = true;
						break;
					}
				}
			}

			//if caller is member of neither denied roles nor allowed roles, then deny caller's access
			if (allowed == false)
			{
				throw new System.Security.SecurityException("Access denied to account " + Thread.CurrentPrincipal.Identity.Name);
			}
		}
	}

	/// <summary>
	/// A sampel authorization provider that shows how to create one to
	/// be used with SAF.Authorization.
	/// </summary>
	public class WindowsAuthorizationProvider : IAuthorizationProvider
	{
		public WindowsAuthorizationProvider()
		{
			
		}
		/// <summary>
		/// It is called by the SAFSecurityPermission.  It performs the
		/// membership check on caller's current principal (principal must be
		/// WindowsPrincipal)
		/// </summary>
		/// <param name="allowedRoles">string array containing the roles allowed</param>
		/// <param name="deniedRoles">string array containing the roles denied</param>
		public void Authorize(string[] allowedRoles, string[] deniedRoles)
		{
			bool allowed = false;
			WindowsPrincipal wp = null;
			//ensure the current thread is WindowsPrincipal
			//if not, get a the windows principal for the current caller 
			if(Thread.CurrentPrincipal.GetType() != typeof(WindowsPrincipal))
			{
				//create a new windows principle representing the caller
				wp = new WindowsPrincipal(WindowsIdentity.GetCurrent());			
			}
			else
			{
				//otherwise, retrieve the existing windows principal from the current thread.
				wp = (WindowsPrincipal)Thread.CurrentPrincipal;
			}

			//check if the caller is member of denied roles first since
			//denied role overrides the allowed roles
			if (deniedRoles != null)
			{
				foreach (string role in deniedRoles)
				{
					//if the caller is member of denied role, reject the call by throwing exception
					if (wp.IsInRole(role))
					{
						throw new System.Security.SecurityException("Access denied to account " + Thread.CurrentPrincipal.Identity.Name);
					}
				}
			}

			//check if caller is member of allowed roles
			if (allowedRoles != null)
			{
				foreach (string role in allowedRoles)
				{
					if (wp.IsInRole(role))
					{
						allowed = true;
						break;
					}
				}
			}
			//if caller is member of neither allowed role nor denied role, reject the call.
			if (allowed ==false)
			{
				throw new System.Security.SecurityException("Access denied to account " + Thread.CurrentPrincipal.Identity.Name);
			}
		}
	}

}
