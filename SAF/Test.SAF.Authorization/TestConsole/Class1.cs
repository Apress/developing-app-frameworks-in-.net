using System;
using System.Security.Principal;
using System.Security.Permissions;
using SAF.Authorization;
using System.Threading;
using System.Security;

namespace TestConsole
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// This demo show how to use SAF.Authorization service in the applicaiton
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			AuthorizationTest at = new AuthorizationTest();
			//attach an principal object to the thread
			Thread.CurrentPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			try
			{
				Console.WriteLine("Enter the MySecureAllowedMethod: ");
				//access the method 
				at.MySecureAllowedMethod("ok");

				Console.WriteLine("Enter the MySecureDeniedMethod: ");
				//access the method
				at.MySecureDeniedMethod();
			}
			catch (SecurityException se)
			{
				Console.WriteLine(se.Message);
			}
			Console.WriteLine("Press enter to exit");
			Console.ReadLine();
		}
	}

	/// <summary>
	/// It contains the methods marked with SAFSecurityAttribute tag
	/// </summary>
	public class AuthorizationTest
	{
		//with the SAFSecurityAttribute, the SAFSecurityPermission takes over the task
		//of access checking of the caller.
		//Refer to the "SAF.Authorization" configuraiton file for more detail
		[SAFSecurityAttribute(SecurityAction.Demand, Name="MyAssembly.MyType.MyMethod1")]
		public void MySecureAllowedMethod(string s)
		{
			Console.WriteLine("it works!\n");
		}

		[SAFSecurityAttribute(SecurityAction.Demand, Name="MyAssembly.MyType.MyMethod2")]
		public void MySecureDeniedMethod()
		{
			Console.WriteLine("it works!\n");
		}
	}
}
