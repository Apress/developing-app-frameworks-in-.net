using System;

namespace SAF.MessageQueue
{
	/// <summary>
	/// Containing the interface and class referenced in
	/// the message queue impelmentation class
	/// </summary>
	public delegate void MessageArrivalHandler (Message m, string queueName);
	
	/// <summary>
	/// interface that every message queue implementation must implements
	/// </summary>
	public interface IMessageQueue
	{
		void Send(Message message);
		Message Retrieve();
		event MessageArrivalHandler MessageArrival;
		void Open();
		void Close();
		
	}

	/// <summary>
	/// Interface for MessageQueueManager.  it is used by
	/// client application to interact with MessageQueueManager.
	/// </summary>
	public interface IMessageQueueManager
	{
		void SendMessage(Message message);
		Message RetrieveMessage();
		void RegisterMessageArrivalHanlder(MessageArrivalHandler mah);
		void UnRegisterMessageArrivalHanlder(MessageArrivalHandler mah);
		void OpenConnection();
		void CloseConnection();
	}

	/// <summary>
	/// A basic Message class, can be extended to create more 
	/// implmentation specific message class
	/// </summary>
	public class Message
	{
		public string Label;
		public object Content;
	}

	
}
