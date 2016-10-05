using System;
using System.Xml;

namespace SAF.Application.Configuration
{
	/// <summary>
	/// DocumentLayerConfiguration  
	/// retrieves document layer related configuration settings.
	/// </summary>
	public class DocumentLayerConfiguration
	{
		private XmlNode configXml;
		public DocumentLayerConfiguration(XmlNode configData)
		{
			configXml = configData;
		}

		/// <summary>
		/// Get an instance of specific document layer 
		/// </summary>
		/// <param name="name">name of the document layer</param>
		/// <returns>an new instance of the document layer.</returns>
		public object GetDocumentLayerByName(string name)
		{
			//retrieve the configuration for a given document layer.
			XmlNode layerXml = configXml.SelectSingleNode("DocumentLayer[@name='" + name + "']");
			XmlNode firstLayer = layerXml.SelectSingleNode("Layer");
			string typeInfo = firstLayer.Attributes["type"].Value;
			Type type = Type.GetType(typeInfo);
			object[] parameters = new Object[1]{firstLayer};
			//create an instance of document layer dynamically.
			object Layer = Activator.CreateInstance(type,parameters);
			return Layer;
		}
	}
}
