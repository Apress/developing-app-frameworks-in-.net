using System;
using System.Threading;
using System.Xml;
using SAF.Utility;

namespace SAF.WindowsService
{
	/// <summary>
	/// SecuritySwitchThread is a thread wrapper class. It allows
	/// developers to customize the way a new thread is started and 
	/// ThreadStart object is invoked.
	/// </summary>
	public class SecuritySwitchThread
	{
		private ThreadStart serviceDelegate;
		private XmlNode runAs;
		private Thread newT;

		/// <summary>
		/// The constructor takes more than one parameter
		/// as oppose to only one parameter is the bare bone Thread
		/// class. This allow we to perform some additional service
		/// based on the additional parameters, in our case, we will pass
		/// the configuration data that contains the user account information
		/// which is later used to perform the thread's security switch
		/// </summary>
		/// <param name="start">the ThreadStart delegate for the target method</param>
		/// <param name="xml">the configuraiton data contains the user account information</param>
		public SecuritySwitchThread (ThreadStart start, XmlNode xml)
		{
			serviceDelegate = start;
			runAs = xml;
			//create a new thread that calls the WrappingMethod
			newT = new Thread(new ThreadStart(WrappingMethod));
		}
		public void Start()
		{
			//start the thread.
			newT.Start();
		}

		/// <summary>
		/// WrappingMethod wraps performs the security
		/// switch and then invokes the ThreadStart delegate.
		/// </summary>
		private void WrappingMethod()
		{
			//retrieve the user account information to which thread is
			//switched.
			bool inheritIdentity = Boolean.Parse(runAs.Attributes["InheritIdentity"].Value);
			if (inheritIdentity == false)
			{
				string userid = runAs.SelectSingleNode("User").InnerText;
				string password= runAs.SelectSingleNode("Password").InnerText;
				string domain = runAs.SelectSingleNode("Domain").InnerText;
				//call the utility class to switch the current thread's security context.
				SecurityUtility su = new SecurityUtility();
				su.Switch(userid, password,domain);
			}
			//invoke the ThreadStart delegate object passed in 
			//on the class constructor
			serviceDelegate();
		}

		/// <summary>
		/// BaseThread property provide access to the 
		/// underlying thread, which allow external program
		/// to control the thread.
		/// </summary>
		public Thread BaseThread
		{
			get
			{
				return newT;
			}
		}
	}
}
