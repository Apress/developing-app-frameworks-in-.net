using System;
using System.Xml;
using System.Messaging;
using SAF.Configuration;
using System.Configuration;
using System.Collections;
using System.Threading;

namespace SAF.MessageQueue
{
	/// <summary>
	/// A sample implementation of IMessageQueue for MSMQ technology.
	/// It allows developers to send, retrieve messages from MSMQ and register
	/// event for new message arrivals.
	/// </summary>
	public class MSMQ : IMessageQueue
	{
		private int sleepTime;
		private string formatName;
		private ConfigurationManager cm;
		private static MessageArrivalHandler handler;
		private string queueName;
		private static bool queueListenerStarted = false;
		private ArrayList supportedTypes = new ArrayList();
		private System.Messaging.MessageQueue mq;

		/// <summary>
		/// Constructor that retrieve the queue related information 
		/// for MessageQueueConfiguration object
		/// </summary>
		/// <param name="queueName">name of the queue</param>
		public MSMQ(string queueName)
		{
			cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			XmlNode queueInfo = cm.MessageQueueConfig.RetrieveQueueInformation("*[@name='" + queueName + "']" );
			formatName = queueInfo.SelectSingleNode("FormatName").InnerText;
			sleepTime =Int32.Parse(queueInfo.SelectSingleNode("SleepTime").InnerText);
			this.queueName = queueName;
			//supportedTypes is used to provide information to System.Messaging.MessageQueue
			//information on how to serialize and deserialize the object sent to and retrieved from
			//the queue.  The default data type is string type.
			supportedTypes.Add(typeof(System.String).ToString());
		}

		/// <summary>
		/// allows developers to add additional type information
		/// for serialization and deserialzation of the messsage
		/// </summary>
		/// <param name="typeName">the data type name</param>
		public void AddSupportedType(string typeName)
		{
			supportedTypes.Add(typeName);
		}

		/// <summary>
		/// event for arrival messages to the queue
		/// </summary>
		public event MessageArrivalHandler MessageArrival
		{
			//when new handler is register for the event, start the listener
			//if it is not yet started
			add
			{
				handler += value;
				if (queueListenerStarted != true)
				{
					//create a new thread to listen on the queue
					ThreadStart ts = new ThreadStart(StartQueueListener);
					Thread t = new Thread(ts);
					t.Start();
				}
			}
			remove
			{
				handler -= value;
				//stop the listener if no handler is listed
				if (handler == null || handler.GetInvocationList().Length <= 0)
				{
					StopQueueListener();
				}
			}
		}
		/// <summary>
		/// Sends the message to the MSMQ
		/// </summary>
		/// <param name="m">message object</param>

		public void Send(SAF.MessageQueue.Message m)
		{
			//set type information queue can use to serialize the message
			((XmlMessageFormatter)mq.Formatter).TargetTypeNames = (string[])(supportedTypes.ToArray(typeof(System.String)));
			mq.Send(m.Content,m.Label);

		}

		/// <summary>
		/// Retrieve the message from the MSMQ
		/// </summary>

		/// <returns>retrieved message object from the queue</returns>
		public SAF.MessageQueue.Message Retrieve()
		{
			//set type information queue can use to deserialize the message
			((XmlMessageFormatter)mq.Formatter).TargetTypeNames = (string[])(supportedTypes.ToArray(typeof(System.String)));
		
			System.Messaging.Message message = mq.Receive();
			SAF.MessageQueue.Message safMessage = new SAF.MessageQueue.Message();
			safMessage.Label = message.Label;
			safMessage.Content = message.Body;
			return safMessage;
		}

		
		/// <summary>
		/// open the connection to the message queue
		/// </summary>
		public void Open()
		{
			 mq = new System.Messaging.MessageQueue(formatName);
		}

		/// <summary>
		/// close the connection to the message queue
		/// </summary>
		public void Close()
		{
			if (mq != null)
			{
				mq.Close();
				mq.Dispose();
			}
		}

		/// <summary>
		/// Start the listen to the queue for incoming messages and 
		/// notifiy the handlers as new messges arrive
		/// </summary>
		private void StartQueueListener()
		{
			//create a separate connection to the message queue
			System.Messaging.MessageQueue listenermq = new System.Messaging.MessageQueue(formatName);
			((XmlMessageFormatter)listenermq.Formatter).TargetTypeNames = (string[])(supportedTypes.ToArray(typeof(System.String)));
			System.Messaging.Message message = null;
			queueListenerStarted = true;
			try
			{
				//listen to the queue continusly through loop
				while (queueListenerStarted == true)
				{
					System.Threading.Thread.Sleep(sleepTime);
					if (handler.GetInvocationList().Length > 0)
					{
						//this is a call that will block the thread if no
						//message is in the queue.
						message = listenermq.Receive();
						SAF.MessageQueue.Message safMessage = new SAF.MessageQueue.Message();
						safMessage.Label = message.Label;
						safMessage.Content = message.Body;
						//fire the event
						handler(safMessage,queueName);
					}

				}
			}
			finally
			{
				//close the connetion
				listenermq.Close();
			}
		}

		/// <summary>
		/// stop the listener
		/// </summary>
		private void StopQueueListener()
		{
			queueListenerStarted = false;
		}

		

	}
}
