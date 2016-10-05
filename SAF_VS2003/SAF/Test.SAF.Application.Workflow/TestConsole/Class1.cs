using System;
using SAF.Application.Configuration;
using SAF.Application.DocumentLayer;
using System.Configuration;
using System.IO;
using System.Security.Principal;

namespace TestConsole
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// Demo that shows how Components and Visitor object
		/// are used together to provide the process flow and coordination logic
		/// of certain tasks
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//read the customer order
			StreamReader sr = null;
			string content=null;
			try
			{
				sr = new StreamReader("..\\..\\OrderSample.xml");
				content = sr.ReadToEnd();
			}
			finally
			{
				sr.Close();
			}
			
			GenericIdentity gi = new GenericIdentity("CustomerA");
			GenericPrincipal gp = new GenericPrincipal(gi,null);
			//create the document object
			IDocument doc = new Document(gp,content,null);

			//load the configuration obect for the workflow
			SAF.Application.Configuration.ConfigurationManager cm =null;
			cm = (ConfigurationManager)ConfigurationSettings.GetConfig("MyApplication");
		
			//get the inital documet layer
			IDocumentLayer layer = (IDocumentLayer)cm.DocumentLayerConfig.GetDocumentLayerByName("PurchaseOrderWorkFlow");
			//start process the document by calling the ProcessDocument on the inital layer.
			//For this perticular example, the last document layer is DocumentWorkFlowLayer object
			//which will trigger the work flow defined in the GenericPurchaseOrderVisitor class
			IDocument response = layer.ProcessDocument(doc);
			//display potential response document
			if (response != null)
			{
				Console.WriteLine(">>>>>>>>>>>>>Repsonse document from " + response.Sender.Identity.Name + " has arrived <<<<<<<<<<<<"); 
				Console.WriteLine("Response Document is: \n " + response.Content + "\n");
			}

			//change the cost of some product to greater than $100 and run this demo again
			//to see how the workflow is changed.

		}
	}
}
