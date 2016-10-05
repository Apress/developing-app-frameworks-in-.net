using System;
using SAF.MessageQueue;

namespace TestConsole
{
	/// <summary>
	/// Demo application to show how to use SAF.MessageQueue service
	/// </summary>
	class Class1
	{
		/// <summary>
		///Demo application to show how to use SAF.MessageQueue service.  Please
		///refer to configuration file to determine what message queues need 
		///to be setup on MSMQ and MQSeries in order to run this demo.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//create a new MSMQ implementation object
			IMessageQueue msmq = new MSMQ("QueueA");
			//bind the implementation to the MessageQueueManager
			IMessageQueueManager mqm = new MessageQueueManager(msmq);

			//create a new message object
			Message m = new Message();
			m.Label = "test";
			m.Content ="this is test";
			try
			{
				mqm.OpenConnection();
				mqm.SendMessage(m);
				Message retrievedMessage = mqm.RetrieveMessage();
				//send two more method the queue for MessageArrival event test later
				mqm.SendMessage(m);
				mqm.SendMessage(m);
			}
			finally
			{
				mqm.CloseConnection();
			}

			//register the client with the message arrive
			mqm.RegisterMessageArrivalHanlder(new MessageArrivalHandler(RecieveMessage));

			//create a new MQSeries implementation
			IMessageQueue mqseries = new MQSeries("QueueB");
			//bind implementation object to the MessageQueueManager
			mqm = new MessageQueueManager(mqseries);
			try
			{
				mqm.OpenConnection();
				mqm.SendMessage(m);
				Message retrievedMessage = mqm.RetrieveMessage();
			}
			finally
			{
				mqm.CloseConnection();
			}


			Console.ReadLine();
		}

		/// <summary>
		/// The method that will be called when message arrives at the queue
		/// </summary>
		/// <param name="message">message object</param>
		/// <param name="queueName">name of queue</param>
		public static void RecieveMessage(Message message, string queueName)
		{
			Console.WriteLine("Received from queue listener: ");
			Console.WriteLine(message.Content.ToString());
			Console.WriteLine(queueName);
		}
	}
}
