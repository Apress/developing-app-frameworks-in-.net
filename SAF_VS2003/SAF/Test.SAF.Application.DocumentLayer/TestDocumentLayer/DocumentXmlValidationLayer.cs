using System;
using System.Xml;
using System.Xml.Schema;
using SAF.Application.DocumentLayer;

namespace TestDocumentLayer
{
	/// <summary>
	/// DocumentXmlValidationLayer shows a sample implementation of
	/// document layer that validate whether the document is compliant with 
	/// the predefined schemas.
	/// </summary>
	public class DocumentXmlValidationLayer : IDocumentLayer
	{
		private IDocumentLayer next;
		private string incomingSchema ;
		private string outgoingSchema;
		private XmlValidatingReader xmlValidatingReader;

		public DocumentXmlValidationLayer()
		{
		}

		public DocumentXmlValidationLayer(XmlNode configXml)
		{
			XmlNode node = configXml.SelectSingleNode("Layer");
			//retrieve the schema information.
			incomingSchema = configXml.SelectSingleNode("Config").Attributes["incomingSchema"].Value;
			outgoingSchema = configXml.SelectSingleNode("Config").Attributes["outgoingSchema"].Value;

			if (node != null)
			{
				Type type = Type.GetType(node.Attributes["type"].Value);
				object[] parameters= new Object[1]{node};
				next = (IDocumentLayer)Activator.CreateInstance(type,parameters);

			}
		}

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
		public IDocument ProcessDocument(IDocument doc)
		{
			ValidateIncomingDocument(doc);
			if (Next != null)
			{
				doc = Next.ProcessDocument(doc);
			}
			if (doc != null)
			{
				ValidateOutgoingDocument(doc);
			}
			return doc;
		}
		/// <summary>
		/// Ensure the incoming document is compliant with schema
		/// </summary>
		/// <param name="doc">incoming document</param>
		private void ValidateIncomingDocument(IDocument doc)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(incomingSchema);
			xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
			xmlValidatingReader.ValidationType = ValidationType.Schema;
			Validate();
		}

		/// <summary>
		/// Ensure the outgoing document is compliant with schema
		/// </summary>
		/// <param name="doc">outgoing document</param>
		private void ValidateOutgoingDocument(IDocument doc)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(outgoingSchema);
			xmlValidatingReader = new XmlValidatingReader(xmlTextReader);
			xmlValidatingReader.ValidationType = ValidationType.Schema;
			Validate();
		}

		private void Validate()
		{
			try
			{
				while (xmlValidatingReader.Read()){}
			}
			catch (Exception e)
			{
				Console.WriteLine ("Exception: " + e.ToString());
				throw e;
			}
		}

	}
}
