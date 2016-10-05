using System;
using System.Xml;
using System.Configuration;
using SAF.Application.Configuration;
using SAF.Application.Workflow;
using System.Collections;
using SAF.Application.DocumentLayer;
using System.Security.Principal;

namespace TestWorkFlow
{
	/// <summary>
	/// GenericPurchaseOrderVisitor shows a sample visitor class which 
	/// contain the coordination logic among different components or processing units.
	/// </summary>
	public class GenericPurchaseOrderVisitor :  IPurchaseOrderVisitor
	{
		private ConfigurationManager cm;
		private WorkFlowConfiguration wc;
		/// <summary>
		/// Constrcutor to initialize the configuration object
		/// </summary>
		public GenericPurchaseOrderVisitor()
		{
			cm = (ConfigurationManager)ConfigurationSettings.GetConfig("MyApplication");
			wc = cm.WorkFlowConfig;
		}

		/// <summary>
		/// Set up the first processing unit for the workflow
		/// </summary>
		/// <param name="ipu"></param>
		public void Visit (InitialComponent ipu)
		{
			ipu.NextComponent = new SendConfirmationEmail();
		}

		/// <summary>
		/// sends a confirmation email to
		/// customers and then set the next processing for order processing
		/// </summary>
		/// <param name="sce">SendConfirmationEmail processing unit</param>
		public void Visit(SendConfirmationEmail sce)
		{
			string customer = sce.Request.Sender.Identity.Name;
			string email = wc.GetCustomerEmail(customer);
			string content = "Your purchase order with following content has been received : \n ";
			//send the email to customer
			sce.CustomerEmail = email;
			sce.MessageContent = content + sce.Request.Content;
			sce.SendEmail();

			//set the next processing unit after the confirmation email has been sent
			sce.NextComponent = new ProcessCustomerOrder();
		}

		/// <summary>
		/// Process the customer order and then set the next processing unit 
		/// depending on the outcome of the order processing
		/// </summary>
		/// <param name="pco">ProcessCustomerOrder processing unit</param>
		public void Visit(ProcessCustomerOrder pco)
		{
			//check if any product is creater than $100
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(pco.Request.Content);
			XmlNodeList nodes = doc.SelectNodes("//*[@cost > 100]");
			//make a decision on what is the next component or processing unit is called
			if (nodes.Count > 0 )
			{
				if (pco.Request.AdditionalData == null)
				{
					pco.Request.AdditionalData = new Hashtable();
					pco.Request.AdditionalData.Add("ErrorDescription","Unable to process product that costs more than $100");
				}
				//set the next processing unit to alert the account manager if error occur
				pco.NextComponent = new AlertAccountManager();
				
			}
			else
			{
				//code here to perform some real work on the order....
				//otherwise, forward the order to the next unit which 
				//will send a response to the customer.
				pco.NextComponent = new SendResponseToCustomer();
			}
		}

		/// <summary>
		/// Display the message about the failed order on the console
		/// </summary>
		/// <param name="sce">AlertAccountManager processing unit</param>
		public void Visit(AlertAccountManager sce)
		{
			Console.WriteLine("An error occured while processing the following document: \n");
			Console.WriteLine(sce.Request.Content + "\n");
			Console.WriteLine("The cause : " + sce.Request.AdditionalData["ErrorDescription"].ToString()); 
		}

		/// <summary>
		/// Returns a response document containing the status information for the customer order
		/// </summary>
		/// <param name="sce">SendResponseToCustomer processing unit</param>
		public void Visit(SendResponseToCustomer sce)
		{
			//add some code here to generate the response document
			string content = "<ns0:Confirmations xmlns:ns0=http://CBRSchemas.Confirmation>" +
								"<Order id=100 Status=OK />" + 
								"<Order id=200 Status=OK />" + 
								"</ns0:Confirmations>";
			GenericIdentity gi = new GenericIdentity("ServerProviderX");
			GenericPrincipal gp = new GenericPrincipal(gi,null);		
			Document response = new Document(gp,content,null);
			//set the response document
			sce.Response = response;
		}
	
	}

	/// <summary>
	/// The interface that is used defined possible process flow inside 
	/// visitor class
	/// </summary>
	public interface IPurchaseOrderVisitor : IVisitor
	{
		void Visit(InitialComponent ipu);
		void Visit(SendConfirmationEmail sce);
		void Visit(ProcessCustomerOrder pco);
		void Visit(AlertAccountManager aam);
		void Visit(SendResponseToCustomer sce);
	}
}
