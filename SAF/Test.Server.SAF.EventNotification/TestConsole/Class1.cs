using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using SAF.EventNotification;
using SAF.Configuration;
using System.Configuration;

namespace TestConsole
{
	/// <summary>
	/// This console application show how to install EventNotification service on the event server.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{

			ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			EventNotificationConfiguration enc = cm.EventNotificationConfig;
			//set up the network channel for accepting calls from event clients and making calls
			//to the event clients.
			HttpChannel channel = new HttpChannel(Int32.Parse(enc.GetPortNumber()));
			ChannelServices.RegisterChannel(channel);
			//register the remote object type.
			WellKnownServiceTypeEntry wste= new WellKnownServiceTypeEntry(typeof(EventServer),enc.GetObjectUri(),WellKnownObjectMode.Singleton);
			RemotingConfiguration.ApplicationName = enc.GetApplicationName();
			RemotingConfiguration.RegisterWellKnownServiceType(wste);
			Console.WriteLine("Press Enter to exit");
			Console.ReadLine();


		}
	}
}
