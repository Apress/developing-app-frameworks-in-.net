using System;
using SAF.EventNotification;
using SAF.Configuration;
using System.Configuration;


namespace TestConsole
{
	/// <summary>
	/// The console application shows how to use EventNotification service.
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
			//retrieve the url of the centralized event server
			string url = cm.EventNotificationConfig.GetEventServerUrl();

			//create an event client.
			EventClient ec= new EventClient(url);
			//event client subscribes some event.
			ec.SubscribeEvent("test1",new EventClient.EventProcessingHandler(Test1_EventReceiver));
			ec.SubscribeEvent("test2",new EventClient.EventProcessingHandler(Test2_EventReceiver));
	
			//event client publishes or raises some event 
			ec.RaiseEvent("test1","this is test1 event");
			ec.RaiseEvent("test2","this is test2 event");
	
			Console.ReadLine();
		}

		//this target method will be invoked when "test1" event occurs
		static void Test1_EventReceiver(string sender, object content)
		{
			Console.WriteLine("Event Received! -- " + content.ToString());
		}

		//this target method will be invoked when "test2" event occurs.
		static void Test2_EventReceiver(string sender, object content)
		{
			Console.WriteLine("Event Received! -- " + content.ToString());
		}

	}

	

}
