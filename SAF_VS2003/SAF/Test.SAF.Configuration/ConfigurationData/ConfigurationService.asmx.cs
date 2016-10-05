using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web;
using System.Web.Services;

namespace ConfigurationData
{
	/// <summary>
	/// Summary description for Service1.
	/// </summary>
	public class ConfigurationService : System.Web.Services.WebService
	{
		public ConfigurationService()
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}

		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

	
		/// <summary>
		/// the method that returns the configuration information stored in 
		/// the centralized location via web service.
		/// </summary>
		[WebMethod]
		public string GetConfiguration(string section, string environment)
		{
			string qa = @"<Application.MessageQueue><CustomerQueue>Direct=OS:.\customer</CustomerQueue><AccountQueue>Direct=OS:.\account</AccountQueue></Application.MessageQueue>";
			string test = @"<Application.MessageQueue><CustomerQueue>Direct=OS:.\test_customer</CustomerQueue><AccountQueue>Direct=OS:.\test_account</AccountQueue></Application.MessageQueue>";
			if (environment == "QAEnvironment" && section == "Application.MessageQueue")
			{
				return qa;
			}
			else
			{
				return test;
			}
		}
	}
}
