using System;
using System.Xml;
using SAF.Configuration;
using System.Configuration;
using MQAX200;
using System.Threading;

namespace SAF.MessageQueue
{
	/// <summary>
	/// A sample implementation of IMessageQueue for MQSeries technology.
	/// It allows developers to send, retrieve messages from MQSeries and register
	/// event for new message arrivals.
	/// </summary>
	public class MQSeries : IMessageQueue
	{
		private ConfigurationManager cm;
		private MessageArrivalHandler handler;
		private string queueManager;
		private string QueueName;
		private MQQueue queue;
		private MQSession queueSession;
		private MQQueueManager mqm;
		private bool queueListenerStarted;
		private int sleepTime;


		/// <summary>
		/// Constructor that retrieve the queue related information 
		/// for MessageQueueConfiguration object
		/// </summary>
		/// <param name="queueName">the name of the queue</param>
		public MQSeries(string queueName)
		{
			cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			XmlNode queueInfo = cm.MessageQueueConfig.RetrieveQueueInformation("*[@name='" + queueName + "']" );
			queueManager = queueInfo.SelectSingleNode("QueueManager").InnerText;
			QueueName = queueInfo.SelectSingleNode("QueueName").InnerText;
			sleepTime = Int32.Parse(queueInfo.SelectSingleNode("SleepTime").InnerText);
			queueSession = new MQSessionClass();
		}

		/// <summary>
		/// send the message to the MQSeries's queue
		/// </summary>
		/// <param name="m">a message object to be sent</param>

		public void Send(Message m)
		{
			//create a new MQSeries message
			MQMessage message = (MQMessage)queueSession.AccessMessage();
			message.WriteString(m.Content.ToString());
			MQPutMessageOptions messageOption = (MQPutMessageOptions)queueSession.AccessPutMessageOptions();
		
			//send the message to the MQSeries queue
			queue.Put(message,messageOption);

		
			
		}

		/// <summary>
		/// Retrieve message from the MQSeries's queue
		/// </summary>
		/// <returns></returns>
		public Message Retrieve()
		{
			//create a new message
			MQMessage message  = (MQMessage)queueSession.AccessMessage();
			MQGetMessageOptions messageOption = (MQGetMessageOptions)queueSession.AccessGetMessageOptions();
	
			//fill the new message object with message from he queue
			//unlike MSMQ, GET is not a blocking call, instead, it raise
			//an exception if trying to retrieve message from a queue that is emtpy.
			queue.Get(message,messageOption,System.Reflection.Missing.Value);
	
		
			//create a new message object that contains the 
			//message from the queue.
			Message m = new Message();
			m.Content = message.ReadString(message.MessageLength);
			m.Label = message.MessageId;
			return m;
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
		/// Start the listen to the queue for incoming messages and 
		/// notifiy the handlers as new messges arrive
		/// </summary>
		public void StartQueueListener()
		{
			//create a separate connection to the message queue
			queueListenerStarted = true;
			MQQueueManager listenermqm = (MQQueueManager)queueSession.AccessQueueManager(queueManager);
			MQQueue listenerqueue =(MQQueue)mqm.AccessQueue(QueueName,(int)MQ.MQOO_INPUT_AS_Q_DEF + (int)MQ.MQOO_OUTPUT,"","","");
			listenerqueue.Open();
			try
			{
				MQMessage message = (MQMessage)queueSession.AccessMessage();
				MQGetMessageOptions messageOption = (MQGetMessageOptions)queueSession.AccessGetMessageOptions();
				while(queueListenerStarted == true)
				{
					System.Threading.Thread.Sleep(sleepTime);
					if (handler.GetInvocationList().Length > 0)
					{
						try
						{
							//GET will raise an exception if no message is in the queue.
							//we want to keep listening despite of the exception, see exception block 
							//for detail
							listenerqueue.Get(message,messageOption,System.Reflection.Missing.Value);
							SAF.MessageQueue.Message safMessage = new SAF.MessageQueue.Message();
							safMessage.Label = message.MessageId;
							safMessage.Content = message.ReadString(message.MessageLength);
							//fire the event
							handler(safMessage,QueueName);
						}
						catch (System.Runtime.InteropServices.COMException ex)
						{
							//-2147467259 represents the error code for retrieving 
							//message from an empty queue. do nothing if gotting this error code.
							if (ex.ErrorCode != -2147467259)
							{
								throw ex;
							}
						}
					}
				}

			}
			finally
			{
				//close the connetion
				listenerqueue.Close();
				listenermqm.Disconnect();
			}
		}

		/// <summary>
		/// stop the listener
		/// </summary>
		public void StopQueueListener()
		{
			queueListenerStarted = false;
			
		}
		
		/// <summary>
		/// open the connection to the message queue
		/// </summary>
		public void Open()
		{
			mqm = (MQQueueManager)queueSession.AccessQueueManager(queueManager);
			queue = (MQQueue)mqm.AccessQueue(QueueName,(int)MQ.MQOO_INPUT_AS_Q_DEF + (int)MQ.MQOO_OUTPUT,"","","");
			queue.Open();

		}

		/// <summary>
		/// close the connection to the message queue
		/// </summary>
		public void Close()
		{
			if (queue != null)
			{
				queue.Close();
			}
			if (mqm != null)
			{
				mqm.Disconnect();
			}
		}
	}
}
