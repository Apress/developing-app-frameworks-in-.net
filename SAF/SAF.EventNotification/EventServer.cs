using System;
using System.Collections;
using SAF.EventNotification;

namespace SAF.EventNotification
{
	/// <summary>
	/// EventServer is responsible for accepting the subscriptions from clients
	/// and notify the clients when information is published to the EventServer.
	/// </summary>
	public class EventServer :  MarshalByRefObject
	{
		//the hastable keep track all the event registered/subscribed
		private Hashtable delegateMap =new Hashtable();
		public EventServer()
		{
		}

		/// <summary>
		/// keep the EventServer live forever
		/// </summary>
		/// <returns></returns>
		public override  object InitializeLifetimeService()
		{
			//object live indefinitely
			return null;
		}
	
		/// <summary>
		/// SubscribeEvent add the client delegate to delegate chain
		/// for a given event name
		/// </summary>
		/// <param name="eventName">event name</param>
		/// <param name="handler">client side delegate</param>
		public void SubscribeEvent(string eventName,EventClient.EventProcessingHandler handler)
		{
			//ensure that only one thread modify the delegate chain at a time.
			lock(this)
			{
				Delegate delegateChain = (Delegate)delegateMap[eventName];
				//check if the delegate chain is null. if null, add the 
				//client side delegate as the initial handler.  Otherwise,
				//add delegate to the chain.
				if (delegateChain == null)
				{
					
					delegateMap.Add(eventName,handler);
				}
				else
				{
					//add the delegate to the chain
					delegateChain = Delegate.Combine(delegateChain,handler);
					//reset the delegate chain in the hashtable
					delegateMap[eventName] = delegateChain;
				}
			}
		}
		
		/// <summary>
		/// UnsubscribeEvent remove an client side delegate from delegate chain
		/// </summary>
		/// <param name="eventName">event name</param>
		/// <param name="handler"></param>
		public void UnSubscribeEvent(string eventName,EventClient.EventProcessingHandler handler)
		{
			//ensure that only one thread modify the delegate chain at a time.
			lock(this)
			{
				Delegate delegateChain = (Delegate)delegateMap[eventName];
				//check if the delegate chain is null. if not null, remove the 
				//client side delegate from the delegate chain.  
				if (delegateChain != null)
				{
					//remove the delegate from the chain
					delegateChain = Delegate.Remove(delegateChain,handler);
					//reset the delegate chain in the hashtable
					delegateMap[eventName] = delegateChain;
				}
			}
		}

		/// <summary>
		/// RaiseEvent invokes the delegate chain of a specific event
		/// </summary>
		/// <param name="eventName">event name</param>
		/// <param name="content">content of the notification</param>
		public void RaiseEvent(string eventName, object content)
		{
			Delegate delegateChain = (Delegate)delegateMap[eventName];
			//retrieve the list of individul delegate from the chain
			IEnumerator invocationEnumerator = delegateChain.GetInvocationList().GetEnumerator();
			//loop through each delegate and invoke it.
			while(invocationEnumerator.MoveNext())
			{
				Delegate handler = (Delegate)invocationEnumerator.Current;
				try
				{
					handler.DynamicInvoke(new object[]{eventName,content});
				}
				catch (Exception ex)
				{
					//if the client who receive event notification is no long
					//available, remove the its delegate 
					delegateChain = Delegate.Remove(delegateChain, handler);
					delegateMap[eventName] = delegateChain;
				}
			}


			
		}
	}
}
