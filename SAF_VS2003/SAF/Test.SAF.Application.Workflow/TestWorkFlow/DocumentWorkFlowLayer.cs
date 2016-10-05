using System;
using System.Xml;
using SAF.Application.Workflow;
using SAF.Application.DocumentLayer;

namespace TestWorkFlow
{
	/// <summary>
	/// A sample show how to triggers the processing units with 
	/// its visitor object in a document layer component
	/// </summary>
	public class DocumentWorkFlowLayer : IDocumentLayer
	{
		private IDocumentLayer next;
		private IComponent nextComponent;
		private IVisitor v;

		public DocumentWorkFlowLayer()
		{
		}
		public DocumentWorkFlowLayer(IDocumentLayer nextLayer)
		{
			Next = nextLayer;
		}

		/// <summary>
		/// constructor that initialize the first processing unit and the visitor class
		/// which contain the process flow and coordination logic used to process the cusotmer orders.
		/// </summary>
		/// <param name="configXml"></param>
		public DocumentWorkFlowLayer(XmlNode configXml)
		{
			XmlNode node = configXml.SelectSingleNode("Layer");
			string initialComponent = configXml.SelectSingleNode("Config/InitialComponent").Attributes["type"].Value;
			string visitor = configXml.SelectSingleNode("Config/Visitor").Attributes["type"].Value;

			//use reflect to create the processing unit and visitor object
			nextComponent = (IComponent)Activator.CreateInstance(Type.GetType(initialComponent),null);
			v = (IVisitor)Activator.CreateInstance(Type.GetType(visitor),null);

			//set the Next property to the next document layer if there is one.
			if (node != null)
			{
				Type type = Type.GetType(node.Attributes["type"].Value);
				object[] parameters= new Object[1]{node};
				next = (IDocumentLayer)Activator.CreateInstance(type,parameters);

			}
		}
		/// <summary>
		/// interface property implementation
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
		/// <param name="doc">document containing the customer order</param>
		/// <returns>response document</returns>
		public IDocument ProcessDocument(IDocument doc)
		{
			IDocument request = null;
			IDocument response = null;
			nextComponent.Request = doc;	

			//Start each processing unit by calling Accept(v) method
			while (nextComponent != null)
			{
				//trigger the process flow logics
				nextComponent.Accept(v);
				request = nextComponent.Request;
				response = nextComponent.Response;
				//set the next proessing unit
				nextComponent = nextComponent.NextComponent;
				//if this is the last processing unit, retrieve 
				//the request and response document
				if (nextComponent != null)
				{
					nextComponent.Request = request;
					nextComponent.Response = response;

				}
			}
			
			//if next document layer exist, proceed with the next layer.
			if (Next != null)
			{
				response = Next.ProcessDocument(response);
			}
			return response;
		}
	}
}
