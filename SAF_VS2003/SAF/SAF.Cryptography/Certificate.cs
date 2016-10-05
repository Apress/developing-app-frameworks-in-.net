using System;
using Microsoft.Web.Services.Security;
using Microsoft.Web.Services.Security.X509;

namespace SAF.Cryptography
{
	/// <summary>
	/// It provides certificate search ability with 
	/// a given location and subject.
	/// </summary>
	public class Certificate
	{
		public Certificate()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Retrieve the X509 certificate for a given subject name and location
		/// </summary>
		/// <param name="location">either CurrentUser store or LocalMachine store</param>
		/// <param name="subject">subject name</param>
		/// <returns>X509Certificate object</returns>
		public static X509Certificate SearchCertificateBySubjectName(string location, string subject)
		{
			X509CertificateStore x509Store = null;
			if (location == "CurrentUser")
			{
				x509Store = X509CertificateStore.CurrentUserStore(X509CertificateStore.MyStore);
			}
			else
			{
				x509Store = X509CertificateStore.LocalMachineStore(X509CertificateStore.MyStore);
			}
			bool open = x509Store.OpenRead();
			X509Certificate certificate =null;
			foreach (X509Certificate cert in x509Store.Certificates)
			{
				if (subject.ToUpper() == cert.GetName().ToUpper())
				{
					certificate = cert; 
					break;
				}
			}
			return certificate;
		}
	}
}

