using System;
using System.Reflection;
using System.Collections;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;
using System.Runtime.Remoting.Activation;

namespace SAF.EventNotification
{
	/// <summary>
	/// EventClient is used to communicate with EventServer.
	/// It provides the client ability to subscribe, unsubscribe and publish
	/// event notification to EventServer
	/// </summary>
	public class EventClient : MarshalByRefObject
	{
		public delegate void EventProcessingHandler(string eventName,object content);
		public delegate void RepeaterHandler(string eventName, object content);
		
		private EventServer es;
		//local hastable to keep track the event that client has subscribed
		private Hashtable repeatDelegate= new Hashtable();
		public EventClient(string url)
		{
			
			//set up the remoting for bilateral communciation
			
			IDictionary props = new Hashtable();
			props["port"] = 0;

			BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
			HttpChannel channel = new HttpChannel(props, clientProvider,null);      
			ChannelServices.RegisterChannel(channel);
			//get the proxy for the remote EventServer object
			es = (EventServer)Activator.GetObject(typeof(EventServer),url);
	
		}
	

		/// <summary>
		/// Subscribe an event on the EventServer
		/// </summary>
		/// <param name="eventName">event name</param>
		/// <param name="s">delegate on the client side that process the event notification</param>
		public void SubscribeEvent(string eventName, EventProcessingHandler s)
		{
			try
			{
				//check if the event has already subscribed by the client
				Delegate handler =(Delegate)repeatDelegate[eventName];
				//if already subscribed, chain up the delegates
				//otherwise, create a new delegate and add it to the repeatDelegate hashtable
				if (handler != null)
				{
					//chain up the delegates together
					handler = Delegate.Combine(handler, s);
					//reset the delegate object in the hashtable
					repeatDelegate[eventName] = handler;

				}
				else
				{
					repeatDelegate.Add(eventName,s);
					EventClient.EventProcessingHandler repeat = new EventClient.EventProcessingHandler(Repeat);
					//subscribe the "repeat" delegate to the EventServer.
					es.SubscribeEvent(eventName,repeat);
				}
			}
			catch (Exception ex)
			{
				 Console.WriteLine(ex.Message);
			}
		}

		/// <summary>
		/// Unsubscribe a event
		/// </summary>
		/// <param name="eventName">event name</param>
		/// <param name="s">delegate on the client side that process the event notification</param>
		public void UnSubscribeEvent(string eventName, EventProcessingHandler s)
		{
			//retrieve the delegate handler from the hashtable
			Delegate handler =(Delegate)repeatDelegate[eventName];
			//check if the handler is null
			//if not null, remove the event processing handler from the chain
			if (handler != null)
			{
				handler = Delegate.Remove(handler, s);
				//reassign the chain back to the hashtable.
				repeatDelegate[eventName] = handler;
			}
		}
		/// <summary>
		/// Repeat method functions as a repeater of the event notification
		/// on the client side. Repeat method is invoke by the EventServer upon the 
		/// arrival of new notificatioin and is responsible for dispatch the notification
		/// on all the "handlers" on the client side
		/// </summary>
		/// <param name="eventName">event name</param>
		/// <param name="content">the content of the notification</param>
		public void Repeat(string eventName, object content)
		{
			//Retrieve the delegate from the hashtable.
			EventProcessingHandler eph = (EventProcessingHandler)repeatDelegate[eventName];
			if (eph !=null)
			{
				//invoke the delegate
				eph(eventName, content);
			}
		}

		/// <summary>
		/// Raise the event
		/// </summary>
		/// <param name="eventName">event name</param>
		/// <param name="content">event content</param>
		public void RaiseEvent(string eventName, object content)
		{
			es.RaiseEvent(eventName,content);
		}
	}
}
