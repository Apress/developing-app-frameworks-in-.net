using System;
using System.Xml;
using SAF.Application.DocumentLayer;
using System.Data;
using System.Data.SqlClient;

namespace TestDocumentLayer
{
	/// <summary>
	/// DocumentLoggingLayer shows a sample implementation of document layer 
	/// that would log incoming and outgoing document to database as documnet flows through the
	/// system.
	/// </summary>
	public class DocumentLoggingLayer : IDocumentLayer
	{
		private IDocumentLayer next;
		private string connString;
		private int documentLoggingID;

		public DocumentLoggingLayer()
		{
		}

		public DocumentLoggingLayer(XmlNode configXml)
		{
			XmlNode node = configXml.SelectSingleNode("Layer");
			connString = configXml.SelectSingleNode("Config").Attributes["connection"].Value;
			if (node != null)
			{
				//retrieve the type information of the document layer.
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

		/// <summary>
		/// Log the incoming and outgoing document to database table
		/// </summary>
		/// <param name="doc">request document</param>
		/// <returns>response document</returns>
		public IDocument ProcessDocument(IDocument doc)
		{
			LogIncomingDocument(doc);
			if (Next != null)
			{
				doc = Next.ProcessDocument(doc);
			}
			if (doc != null)
			{
				LogOutgoingDocument(doc);
			}
			return doc;
		}

		private void LogIncomingDocument(IDocument doc)
		{
			DataHelper dh = new DataHelper(connString);
			documentLoggingID = dh.GetMaxID();
			string sql = "insert into DocumentLogging values (" +
							documentLoggingID + " ,'in','" + doc.Sender.Identity.Name + "','" +
							doc.Content + "',GETDATE())";
							
			dh.ExecuteQuery(sql);

		}

		private void LogOutgoingDocument(IDocument doc)
		{
			DataHelper dh = new DataHelper(connString);
			string sql = "insert into DocumentLogging values (" +
				documentLoggingID + " ,'out','" + doc.Sender.Identity.Name + "','" +
				doc.Content + "',GETDATE())";
							
			dh.ExecuteQuery(sql);
		}
	}

	/// <summary>
	/// Database helper class
	/// </summary>
	public class DataHelper
	{
		private SqlConnection conn;
		public DataHelper(string connstring)
		{
			conn = new SqlConnection(connstring);
		}
		public void ExecuteQuery(string sql)
		{
			try
			{
				conn.Open();
				SqlCommand command = conn.CreateCommand();
				command.CommandText = sql;
				command.ExecuteNonQuery();
			}
			finally
			{
				conn.Close();
			}

		}

		public int GetMaxID()
		{
			int maxid = 0;
			try
			{
				conn.Open();
				SqlCommand command = conn.CreateCommand();
				command.CommandText ="select max(id) from DocumentLogging";
				SqlDataReader dr = command.ExecuteReader();
				while (dr.Read())
				{
					try
					{
						maxid =dr.GetInt32(0);
					}
					catch (System.Data.SqlTypes.SqlNullValueException ex)
					{
						maxid =0;
					}
					break;				
				}
				
				return maxid + 1 ;
			}
			finally
			{
				conn.Close();
			}

		}
	}
}
