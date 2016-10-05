using System;
using System.Xml;
namespace SAF.Configuration
{
	/// <summary>
	/// MessageQueueConfiguration is used to retreive configuration information
	/// for SAF.MessageQueue component
	/// </summary>
	public class MessageQueueConfiguration
	{
		private XmlNode messageQueueXml;
		public MessageQueueConfiguration(XmlNode configData)
		{
			messageQueueXml = configData;
		}

		
		/// <summary>
		/// retrieve the configuration information for a given queue
		/// </summary>
		/// <param name="queueName">name of the queue</param>
		/// <returns>xml contain the configuraiton information of the queue</returns>
		public XmlNode RetrieveQueueInformation(string queueName)
		{
			return messageQueueXml.SelectSingleNode(queueName);
		}


		/// <summary>
		/// Retrieves the IMessageQueue object specified in the configuration file
		/// </summary>
		/// <param name="queueName">name that identities the message queue</param>
		/// <returns>IMessageQueue object</returns>
		public object RetrieveMessageQueueImplementation(string queueName)
		{
			string typeInfo = messageQueueXml.SelectSingleNode(queueName).Attributes["type"].Value;
			Type type = Type.GetType(typeInfo);
			object[] parameters = new Object[1]{RetrieveQueueInformation(queueName)};
			return Activator.CreateInstance(type,parameters);
		}
	}
}
