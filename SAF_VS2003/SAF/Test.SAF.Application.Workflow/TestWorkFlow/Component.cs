using System;
using SAF.Application.Workflow;
using SAF.Application.DocumentLayer;
using System.Web.Mail;

namespace TestWorkFlow
{
	/// <summary>
	/// ComponentHelper is an abstract method that implements
	/// some of the IComponent's interface members.  It reduces the 
	/// amount developers have to write to create the processing unit component
	/// </summary>
	public abstract class ComponentHelper : IComponent
	{
		private IComponent  nextComponent;
		private IDocument request;
		private IDocument response;

		public IDocument Request
		{
			get{return request;}
			set {request = value;}
		}

		public IDocument Response
		{
			get{return response;}
			set{response =value;}
		}

		public IComponent NextComponent
		{
			get{return nextComponent;}
			set{nextComponent = value;}
		}
	
		public abstract void Accept(IVisitor v);
	}

	/// <summary>
	/// acts as an initial processing unit, it determines
	/// what the inital action to take to process the cusotmer order.
	/// </summary>
	public class InitialComponent : ComponentHelper
	{
		public override void Accept(IVisitor v)
		{
			//triggers the coordination logic stored in the visitor object
			((IPurchaseOrderVisitor)v).Visit(this);
		}

	}

	/// <summary>
	/// contains the methods that send out the email with given 
	/// email address and content. Its method is called by the visitor object
	/// </summary>
	public class SendConfirmationEmail : ComponentHelper
	{	public string CustomerEmail;
		public string SMTPServerName;
		public string MessageContent;

		public override void Accept(IVisitor v)
		{
			//triggers the coordination logic stored in the visitor object
			((IPurchaseOrderVisitor)v).Visit(this);	
		}


		/// <summary>
		/// method that perform the email sending.  It is 
		/// called from the Visitor object, 
		/// refer to GenericPurchaseOrderVisitor.Visit(SendConfirmationEmail sce)
		/// for more detail
		/// </summary>
		public void SendEmail()
		{
			try
			{
				SmtpMail.SmtpServer = SMTPServerName;
				MailMessage mm = new MailMessage();
				mm.To = CustomerEmail;
				mm.From = "confirmation@company.com";
				mm.Body = MessageContent;
				SmtpMail.Send(mm);
			}
			catch (Exception ex)
			{
				//ignor the exception if SMTP is setup right on your computer
			}
		}
	}

	/// <summary>
	/// Sample processing unit for processing customer order.
	/// potential methods can be added to provide more complex implementation
	/// </summary>
	public class ProcessCustomerOrder : ComponentHelper
	{
		public override void Accept(IVisitor v)
		{
			//triggers the coordination logic stored in the visitor object
			((IPurchaseOrderVisitor)v).Visit(this);
		}
	}

	/// <summary>
	/// Sample processing unti for alerting the account managers
	/// potential methods can be added to provide more complext implementation
	/// </summary>
	public class AlertAccountManager : ComponentHelper
	{
		public override void Accept(IVisitor v)
		{
			//triggers the coordination logic stored in the visitor object
			((IPurchaseOrderVisitor)v).Visit(this);
		}
	}


	/// <summary>
	/// Sample processing unti for generating and sending response document
	/// potential methods can be added to provide more complext implementation
	/// </summary>
	public class SendResponseToCustomer : ComponentHelper
	{
		public override void Accept(IVisitor v)
		{
			//triggers the coordination logic stored in the visitor object
			((IPurchaseOrderVisitor)v).Visit(this);
		}
	}
}
