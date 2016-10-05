using System;
using System.Threading;


namespace Test.BusinessLibrary
{
	/// <summary>
	/// this demo shows how to use CryptoRemotingClientSink and CryptoRemotingServerSink
	/// together to provide an secure communication environment for .NET remoting
	/// </summary>
	public class SampleBusiness : MarshalByRefObject
	{
		public string SayHelloWorld()
		{
			//retrieve the data slot from the thread which contains the sender's identity information
			LocalDataStoreSlot dataSlot = Thread.GetNamedDataSlot("Identity");
			string sender = (string)Thread.GetData(dataSlot);
			Console.WriteLine("Caller named '" + sender + "' calls the SayHelloWorld() at " + System.DateTime.Now.ToLongTimeString());
			return "Hello World";
		}
	}
}
