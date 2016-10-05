using System;
using TestDocumentLayer;
using SAF.Application.Configuration;
using System.Configuration;
using SAF.Application.DocumentLayer;
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
		/// The demo shows how to use DocumentLayer in the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			StreamReader sr = null;
			string content=null;
			try
			{
				//load the order xml.
				sr = new StreamReader("..\\..\\OrderSample.xml");
				content = sr.ReadToEnd();
			}
			finally
			{
				sr.Close();
			}
			
			GenericIdentity gi = new GenericIdentity("CustomerA");
			GenericPrincipal gp = new GenericPrincipal(gi,null);
			//create a new document object
			Document doc = new Document(gp,content,null);

			SAF.Application.Configuration.ConfigurationManager cm =null;
			cm = (SAF.Application.Configuration.ConfigurationManager)ConfigurationSettings.GetConfig("MyApplication");
			//get the initial document layer object
			IDocumentLayer layer = (IDocumentLayer)cm.DocumentLayerConfig.GetDocumentLayerByName("Special");
			//start processing the document.
			IDocument response = layer.ProcessDocument(doc);
			Console.WriteLine(">>>>>>>>>>>>>Repsonse document from " + response.Sender.Identity.Name + " has arrived <<<<<<<<<<<<"); 
			Console.WriteLine("Response Document is: \n " + response.Content + "\n");

		}
	}
}
