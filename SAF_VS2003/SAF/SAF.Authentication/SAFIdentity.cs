using System;
using System.Security.Principal;

namespace SAF.Authentication
{
	/// <summary>
	/// SAFIdentity encapsulate the identity and the application name information
	/// </summary>

	public  class SAFIdentity : IIdentity
	{
		protected string applicationName;
		private bool isAuthenticated;
		protected string name;


		/// <summary>
		/// this constructor is used exclusively by SAFPrincipal.
		/// If provide a way for SAFPrincipal a way to create SAFIdentity
		/// object with user credential information.  the constructor
		/// is marked internal to avoid develop to call directly.
		/// </summary>
		internal SAFIdentity(string _name, string _applicationName)
		{
			name = _name;
			applicationName = _applicationName;
		}

		protected SAFIdentity(){}

		/// <summary>
		/// ApplicationName provide the name of the application
		/// which will be used later to identity what type of principal object used
		/// to determine the role information
		/// </summary>
		public string ApplicationName
		{
			get	{return applicationName;}
		}
		/// <summary>
		/// IIdentity interface method
		/// </summary>
		public bool IsAuthenticated
		{
			get {return isAuthenticated;}
			set {isAuthenticated = value;}

		}

		/// <summary>
		/// IIdentity interface method
		/// </summary>
		public string Name
		{
			get {return name;}
		}

		/// <summary>
		/// IIdentity interface method
		/// </summary>
		public virtual string AuthenticationType 
		{
			get{return "Basic authentication";}
		}
	}
}
