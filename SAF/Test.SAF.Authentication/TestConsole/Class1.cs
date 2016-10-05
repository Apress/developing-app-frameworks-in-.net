using System;
using SAF.Authentication;
using System.Threading;
using System.Xml.Serialization;
using System.IO;


namespace TestConsole
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The memo show how to use SAF.Authentication service
		/// to "log on" once and have access to many application without 
		/// relogging again.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//Note: modify the user login information to match with that of 
			//you computer.

			//create an SAFIdentity type object, modify the parameters to fit your computer's security enviroment
			ADIdentity adIdentity = new ADIdentity("myuser","mypassword","mylocalcomputer","Application1");
			SAFPrincipal sp = new SAFPrincipal(adIdentity);
			//attach the SAFPrincipal to the thread
			Thread.CurrentPrincipal = sp;
			
			//refer to the configuraiton file for more information how how the 
			//users and applications are mapped 
			//check if the current principal is member of Administrators group 
			bool result1 = sp.IsInRole("Administrators");
			string user1 = sp.Identity.Name;
	
			//switch to another application, then check what the original caller means to them membership-wise.
			sp.SetApplication("Application2");
			bool result2 = sp.IsInRole("Administrators");
			string user2 = sp.Identity.Name;

			//switch to another application, then check what the original caller means to them membership-wise.
			sp.SetApplication("Application1");
			bool result3 = sp.IsInRole("Administrators");
			string user3 = sp.Identity.Name;


	
			Console.ReadLine();
		}
	}
}
