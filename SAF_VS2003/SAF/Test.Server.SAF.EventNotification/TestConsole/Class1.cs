using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using SAF.EventNotification;
using SAF.Configuration;
using System.Configuration;
using System.Collections;
using System.Runtime.Serialization.Formatters;

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
			IDictionary props = new Hashtable();
			props["port"] = Int32.Parse(enc.GetPortNumber());

			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;
			HttpChannel channel = new HttpChannel(props,null,provider);

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
