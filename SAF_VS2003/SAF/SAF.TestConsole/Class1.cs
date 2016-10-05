using System;
using SAF.Utility;

namespace SAF.TestConsole
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//
			// TODO: Add code to start application here
			//
			SecurityUtility su = new SecurityUtility();
			try
			{
				su.Switch("avmin","access","AVANADE-C006T6X");
			}
			catch (Exception ex)
			{
				string x = ex.Message;
			}
		}
	}
}
