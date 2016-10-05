using System;
using System.Xml;
using System.IO;
using System.Security.Principal;
using SAF.Application.DocumentLayer;


namespace TestDocumentLayer
{
	/// <summary>
	/// This class demonstrates what's going on inside a document layer object and
	/// how it process the document.
	/// </summary>
	public class DocumentDoSomeWorkLayer  : IDocumentLayer
	{
		private IDocumentLayer next;

		public DocumentDoSomeWorkLayer()
		{
		}

		public DocumentDoSomeWorkLayer(XmlNode configXml)
		{
			XmlNode node = configXml.SelectSingleNode("Layer");
			if (node != null)
			{
				//retrieve the type information of the document layer from the configuration xml
				Type type = Type.GetType(node.Attributes["type"].Value);
				object[] parameters= new Object[1]{node};
				next = (IDocumentLayer)Activator.CreateInstance(type,parameters);

			}
		}

		/// <summary>
		/// Next Property holds the reference for the next document object.
		/// </summary>
		public IDocumentLayer Next
		{
			get
			{
				return next;
			}
			set
			{
				next = value;
			}
		}

		/// <summary>
		/// ProcessDocument is responsible for process the document.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public IDocument ProcessDocument(IDocument doc)
		{
			//do some serious work here
			Console.WriteLine(">>>>>>>>>>>Request document from " + doc.Sender.Identity.Name + " has arrived<<<<<<<<<<<<");
			Console.WriteLine("Request document is: \n" + doc.Content + "\n");


			StreamReader sr = null;
			string content=null;
			try
			{
				//load the confirmation document.
				sr = new StreamReader("..\\..\\ConfirmationSample.xml");
				content = sr.ReadToEnd();
			}
			finally
			{
				sr.Close();
			}
			
			GenericIdentity gi = new GenericIdentity("ServerProviderX");
			GenericPrincipal gp = new GenericPrincipal(gi,null);
			
			//generate a response document and return it to the caller.
			Document response = new Document(gp,content,null);
			return response;
		}
	}
}
