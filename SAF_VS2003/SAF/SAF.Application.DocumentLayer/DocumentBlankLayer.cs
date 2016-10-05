using System;
using System.Xml;
using SAF.Application.DocumentLayer;

namespace SAF.Application.DocumentLayer
{
	/// <summary>
	/// DocumentBlankLayer is an sample document layer objects that
	/// don't do any task but contain all the elements needed to function as a document
	/// layer object.
	/// </summary>
	public class DocumentBlankLayer :  IDocumentLayer
	{
		private IDocumentLayer next;

		/// <summary>
		/// set up the next document layer in the chain
		/// </summary>
		/// <param name="nextLayer">next document layer</param>
		public DocumentBlankLayer(IDocumentLayer nextLayer)
		{
			Next = nextLayer;
		}

		/// <summary>
		/// Create an object for the next document layer and set it
		/// to the Next property
		/// </summary>
		/// <param name="configXml">configuration data contain the document layers participating the chain</param>
		public DocumentBlankLayer(XmlNode configXml)
		{
			XmlNode node = configXml.SelectSingleNode("Layer");
			if (node != null)
			{
				Type type = Type.GetType(node.Attributes["type"].Value);
				object[] parameters= new Object[1]{node};
				next = (IDocumentLayer)Activator.CreateInstance(type,parameters);

			}
		}
		/// <summary>
		/// interface properties
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
		/// interface method that process the document
		/// </summary>
		/// <param name="doc">the document to be processed on</param>
		/// <returns>document after the processing, or response document</returns>
		public IDocument ProcessDocument(IDocument doc)
		{

			if (Next != null)
			{
				doc = Next.ProcessDocument(doc);
			}
			return doc;
		}
	
	}
}
