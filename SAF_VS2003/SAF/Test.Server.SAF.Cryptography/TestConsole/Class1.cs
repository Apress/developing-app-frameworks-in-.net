using System;
using System.Runtime.Remoting;

namespace Test.Server.SAF.Cryptography
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The demo application set up the server side remoting service for secure communication.
		/// You need to start this console app first before you start Test.Client.SAF.Cryptography project
		/// in order to get the secure remoting to work.
		/// Please refer to SAF.Cryptography section in app.config file for more information on how 
		/// to configure the cryptography of the remoting server sink.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//load the configuration data which will set up the server remoting sink for 
			//secure communication.
			RemotingConfiguration.Configure(@"TestConsole.exe.config");
			Console.WriteLine("press enter to exit");
			Console.ReadLine();
		}
	}
}
