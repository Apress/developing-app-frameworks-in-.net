using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace SAF.Utility
{
	/// <summary>
	/// SecurityUtility provide features such as check if certain window user logon is valid
	/// and switch the current thread's security context to different user account.
	/// </summary>
	public class SecurityUtility
	{
		//declare for p/invoke
		[DllImport(@"advapi32.dll")]
		public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, 
			int dwLogonType, int dwLogonProvider, out System.IntPtr phToken);

		[DllImport(@"Kernel32.dll")]
		public static extern int GetLastError();

		[DllImport(@"advapi32.dll", CharSet=System.Runtime.InteropServices.CharSet.Auto, SetLastError=true)]
		public extern static bool DuplicateToken(IntPtr hToken, 
			int impersonationLevel,  
			ref IntPtr hNewToken);

		private const int LOGON32_LOGON_INTERACTIVE = 2;
		private const int LOGON32_PROVIDER_DEFAULT = 0;
		private const int SecurityImpersonation = 2;
		private WindowsImpersonationContext impersonationContext = null;

		/// <summary>
		/// This method change the thread's runing-as to the new user account
		/// </summary>
		/// <param name="userName">the user account to switch to</param>
		public void Switch (string userName, string password, string domain)
		{
			IntPtr token = IntPtr.Zero;
			WindowsImpersonationContext impersonationContext = null;

			//log on as the give user account
			bool loggedOn = LogonUser(
				// User name.
				userName,
				// Computer or domain name.
				domain,
				password,
				LOGON32_LOGON_INTERACTIVE,   
				LOGON32_PROVIDER_DEFAULT,    
				// The user token for the specified user is returned here.
				out token); 

			if (loggedOn == false)
			{
				throw new System.Security.SecurityException(userName + " logon failed" );
			}	
			IntPtr tokenDuplicate = IntPtr.Zero;
			WindowsIdentity tempWindowsIdentity =null;
			//duplicate the security token
			if(DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != false) 
			{
				tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
				//change the current thread's run-as to the new window identity.
				impersonationContext = tempWindowsIdentity.Impersonate();
			}
			else
			{
				throw new System.Security.SecurityException("Logon use failed");
			}
		}
		/// <summary>
		/// this method reverse the thread's run-as to the original user account
		/// </summary>
		public void UndoSwitch()
		{
			impersonationContext.Undo();
		}

		/// <summary>
		/// Return WindowsIdentity object that represents the user with provided
		/// username/password/domain combination.
		/// </summary>
		/// <param name="userName">user name</param>
		/// <param name="password">user password</param>
		/// <param name="domain">domain or computer name if using local security</param>
		/// <returns></returns>
		public WindowsIdentity LogonUser(string userName, string password, string domain)
		{
			IntPtr token = IntPtr.Zero;
			bool loggedOn = LogonUser(
				// User name.
				userName,
				// Computer or domain name.
				domain,
				password,
				LOGON32_LOGON_INTERACTIVE,   
				LOGON32_PROVIDER_DEFAULT,    
				// The user token for the specified user is returned here.
				out token); 

			if (loggedOn == false)
			{
				throw new System.Security.SecurityException(userName + " logon failed" );
			}
			//create an WindowIdentity object from the newly created token.
			WindowsIdentity newID = new WindowsIdentity(token);
			return newID;
		}

		/// <summary>
		/// Check if username/password/domain combination represent
		/// an valid user logon.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="domain"></param>
		public void ValidateUser(string userName, string password, string domain)
		{
			IntPtr token = IntPtr.Zero;
			bool loggedOn = LogonUser(
				// User name.
				userName,
				// Computer or domain name.
				domain,
				// Password.
				password,
				// LOGON32_LOGON_INTERACTIVE .
				LOGON32_LOGON_INTERACTIVE,   
				// Logon provider = LOGON32_PROVIDER_DEFAULT.
				LOGON32_PROVIDER_DEFAULT,    
				// The user token for the specified user is returned here.
				out token); 

			if (loggedOn  == false)
			{
				throw new System.Security.SecurityException(userName + " logon failed" );
			}
		}



	}
}
