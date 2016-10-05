using System;
using System.Configuration;
using SAF.Configuration;

namespace SAF.MessageQueue
{
	/// <summary>
	/// MessageQueueManager contains the API that developers
	/// would interact with the most. It provide the "bridge" 
	/// to the underlying implementation of message queueing.
	/// This compnoent show most simplified linkage between MessageQueueManager
	/// and underlying Message queue.  The more dissimilar between IMessageManager and the
	/// underlying Message Queue, the more complex MessageQueueManager becomes.
	/// </summary>
	public class MessageQueueManager : IMessageQueueManager
	{ 
		private IMessageQueue mq;
		private ConfigurationManager cm;

		public MessageQueueManager(IMessageQueue messageQueue)
		{
			mq = messageQueue;
		}

		/// <summary>
		/// pass the call the actual message queue implementation
		/// </summary>
		/// <param name="m">message object</param>
		public void SendMessage(Message m)
		{
			mq.Send(m);
		}

	
		/// <summary>
		/// pass the call the actual message queue implementation
		/// </summary>
		/// <returns>the retrieved message from the queue</returns>
		public Message RetrieveMessage()
		{
			return mq.Retrieve();
		}

		/// <summary>
		/// register event hanlder that trigger when new message arrives at the queue
		/// </summary>
		/// <param name="mah">MessageArrivalHandler delegate</param>
		public void RegisterMessageArrivalHanlder(MessageArrivalHandler mah)
		{
			mq.MessageArrival += mah;
		}

		/// <summary>
		/// unregister event handler that trigger when new message arrives at the queue.
		/// </summary>
		/// <param name="mah">MessageArrivalHandler delegate</param>
		public void UnRegisterMessageArrivalHanlder(MessageArrivalHandler mah)
		{
			mq.MessageArrival -= mah;
		}

		/// <summary>
		/// open the connection of the underlying mesage queue
		/// </summary>
		public void OpenConnection()
		{
			mq.Open();
		}

		/// <summary>
		/// close the connection of the underlying message queue
		/// </summary>
		public void CloseConnection()
		{
			mq.Close();
		}

	}

	
}
