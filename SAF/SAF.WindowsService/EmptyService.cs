using System;
using System.IO;
using SAF.Library.WindowsService;
using System.Threading;
using System.Xml;

namespace SAF.WindowsService
{
	/// <summary>
	/// EmptyService is a sample implementation of ISerivce class that is
	/// pluggable to SAF.WindowsService.  When started, EmplyService writes out some text to 
	/// a file every 3 seconds.
	/// </summary>
	public class EmptyService : IService
	{
		private bool continueLoop = true;
		private string filePath;
		public EmptyService()
		{
		}

		public void Initialize(XmlNode configXml)
		{
			filePath = configXml.SelectSingleNode("File").InnerText;
		}

		public void Start()
		{
			StreamWriter sr = null ;
			try
			{
				sr= new StreamWriter(filePath);
				while (continueLoop)
				{
					try
					{
						Thread.Sleep(3000);		
						sr.WriteLine(System.DateTime.Now.ToLongTimeString());
						sr.Flush();
					}
					catch (ThreadInterruptedException){}
	
				}
			}
			finally
			{
				if (sr != null){sr.Close();}
				
			}

		}
		public void Stop()
		{
			continueLoop = false;
			Thread.CurrentThread.Interrupt();

		}
	}
}
