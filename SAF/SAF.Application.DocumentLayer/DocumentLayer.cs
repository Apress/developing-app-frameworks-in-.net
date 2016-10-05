using System;
using System.Security.Principal;
using System.Collections;

namespace SAF.Application.DocumentLayer
{
	/// <summary>
	/// IDocument reprersent the interface of the document that 
	/// will be pass through the application during document processing
	/// </summary>
	public interface IDocument
	{
		IPrincipal Sender
		{
			get;set;
		}
		string Content
		{
			get;set;
		}
		Hashtable AdditionalData
		{
			get;set;
		}
	}

	/// <summary>
	/// IDocumentLayer specify the interface every document layer must implement
	/// in order to participate in the document processing chain.
	/// </summary>
	public interface IDocumentLayer
	{
		/// <summary>
		/// refers to the next document layer object in the chain
		/// </summary>
		IDocumentLayer Next
		{
			get;set;
		}
		/// <summary>
		/// ProcessDocument gives document layer an opportunity to process the document.
		/// </summary>
		IDocument ProcessDocument(IDocument document);
	}

	/// <summary>
	/// Provide an default implementation of document object
	/// </summary>
	public class Document : IDocument
	{
		private IPrincipal sender;
		private string content;
		private Hashtable additionalData = new Hashtable();

		public Document(IPrincipal s, string c, Hashtable a)
		{
			sender = s;
			content = c;
			additionalData = a;
		}
		/// <summary>
		/// represents the sender of the document
		/// </summary>
		public IPrincipal Sender
		{
			get
			{
				return sender;
			}
			set
			{
				sender =value;
			}
		}

		/// <summary>
		/// represents the content of the document
		/// </summary>
		public string Content
		{
			get
			{
				return content;
			}
			set
			{
				content =value;
			}
		}

		/// <summary>
		/// represents additonal data assocated with the document
		/// </summary>
		public Hashtable AdditionalData
		{
			get
			{
				return additionalData;
			}
			set
			{
				additionalData =value;
			}

		}
	}
}
