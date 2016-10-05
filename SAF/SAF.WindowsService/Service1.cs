using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Xml;
using SAF.Library.WindowsService;
using System.Threading;
using SAF.Configuration;
using System.Configuration;

namespace SAF.WindowsService
{
	public class Service1 : System.ServiceProcess.ServiceBase
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		/// 
		private ArrayList threadArray = new ArrayList();
		private ArrayList instanceArray = new ArrayList();
		private System.ComponentModel.Container components = null;

		public Service1()
		{
			// This call is required by the Windows.Forms Component Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitComponent call
		}

		// The main entry point for the process
		static void Main()
		{
			System.ServiceProcess.ServiceBase[] ServicesToRun;
	
			// More than one user Service may run within the same process. To add
			// another service to this process, change the following line to
			// create a second service object. For example,
			//
			//   ServicesToRun = New System.ServiceProcess.ServiceBase[] {new Service1(), new MySecondUserService()};
			//
			ServicesToRun = new System.ServiceProcess.ServiceBase[] { new Service1() };
			System.ServiceProcess.ServiceBase.Run(ServicesToRun);
		}

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			this.ServiceName = "SAF.WindowsService";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		/// <summary>
		/// Set things in motion so your service can do its work.
		/// </summary>
		protected override void OnStart(string[] args)
		{

			
			//obtain the configuration information for SAF.Service
			ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
			SAF.Configuration.ServiceConfiguration serviceConfig = cm.ServiceConfig;
			XmlNode servicesXml = serviceConfig.ServicesXml;
			//loop through service nodes and start them one by one
			foreach (XmlNode node in servicesXml.ChildNodes)
			{	
				try
				{
					string typeInfo;
					//obtain the type information from the xml data
					typeInfo =node.Attributes["type"].Value;
					Type type = Type.GetType(typeInfo);
					IService instance = (IService)Activator.CreateInstance(type);
					//initialize the service
					instance.Initialize(node);
					XmlNode runAs = node.SelectSingleNode("RunAs");
					instanceArray.Add(instance);

					//create SecuritySwitchThread object to process the
					//the service
					ThreadStart ts = new ThreadStart(instance.Start);
					SecuritySwitchThread sst = new SecuritySwitchThread(ts,runAs);
					//start the SecuritySwitchThread's thread.
					sst.Start();
					threadArray.Add(sst.BaseThread);
				}
				catch (Exception ex)
				{
					//write to the event log
				}
			}
		}

		/// <summary>
		/// delegate used when invoke the OnStop method asynchronous during service shut down.
		/// </summary>
		public delegate void OnStopDelegate();

		/// <summary>
		/// Stop this service.
		/// </summary>
		protected override void OnStop()
		{
			foreach (object o in instanceArray)
			{
				try
				{
					IService service = (IService)o;
					if (service !=null)
					{
						//invoke the delegate asynchronous to stop each started service.
						OnStopDelegate osd = new OnStopDelegate(service.Stop);
						osd.BeginInvoke(null,null);
					}
				}
				catch (Exception ex)
				{
					//write to the event log
				}
			}

			//give sometime for the each instance to shut down gracefully
			Thread.Sleep(5000);
			foreach (object o in threadArray)
			{
				try
				{
					Thread t = (Thread)o;
					if (t !=null)
					{
						//if the thread is still live at this point, shut it down forcefully.
						if (t.IsAlive == true)
						{
							t.Abort();
						}
					}
				}
				catch (Exception ex)
				{
					//write to the event log
				}
			}
		}
	}
}
