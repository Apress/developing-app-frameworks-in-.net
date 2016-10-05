using System;
using System.Runtime.Remoting;
using Test.BusinessLibrary;
using SAF.Cryptography;

namespace TestConsole
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// This demo shows how to use CryptoRemotingClientSink and CrytoRemotingServerSink
		/// together to provide an secure communciation environment for .NET remoting, as 
		/// well as use SAF.Crytography to encrypt and decrypt data.  You need to start the
		/// Test.Server.SAF.Cryptography project first in order to test the secure remoting calls.
		/// Please refer to SAF.Cryptography section in app.config file fore more information on how 
		/// to configure the cryptography of the remoting client sink.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//load configuration data
			RemotingConfiguration.Configure(@"TestConsole.exe.config");

			//(1). Test the data encryption/decryption using SAF.Cryptography
			//	(a) test the symmatric cryptography
			string original ="This is a test!" ;
			string encrypted = Encryption.Encrypt(original,"Profile1");
			Console.WriteLine("Encrypted data is : " + encrypted);
			string decrypted = Decryption.Decrypt(encrypted,"Profile1");
			Console.WriteLine("Decrypted data is : " + decrypted);

			//	(b) test the asymmatric cryptography
			byte[] key;
			byte[] iv;
			byte[] signature;
			encrypted = Encryption.Encrypt(original,"Profile2",out key,out iv, out signature);
			Console.WriteLine("Encrypted data is : " + encrypted);
			decrypted = Decryption.Decrypt(encrypted,"Profile3",key,iv,signature);
			Console.WriteLine("Decrypted data is : " + decrypted);


			//(2). Test the secure remoting call via CryptoRemotingClient(Server)Sink.
			//Please refer to configuration file for profile information used for remoting calls.
			//creating the remoting object
			SampleBusiness sb = new SampleBusiness();
			//invoking the secure remoting call.
			Console.WriteLine(sb.SayHelloWorld());
			Console.WriteLine("press enter to exit");
			Console.ReadLine();
		}
	}
}
