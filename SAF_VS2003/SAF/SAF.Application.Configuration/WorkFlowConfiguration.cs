using System;
using System.Xml;

namespace SAF.Application.Configuration
{
	/// <summary>
	/// Provides the configuration object for the application
	/// specific component
	/// </summary>
	public class WorkFlowConfiguration
	{
		private XmlNode configXml;
		public WorkFlowConfiguration(XmlNode configData)
		{
			configXml = configData;
		}
		
		/// <summary>
		/// retrieve the email address for 
		/// a given customer
		/// </summary>
		/// <param name="customer">customer name</param>
		/// <returns>email address</returns>
		public string GetCustomerEmail(string customer)
		{
			XmlNode customers = configXml.SelectSingleNode("Customers");
			string email = customers.SelectSingleNode(customer).Attributes["email"].Value;
			return email;

		}


	}
}
