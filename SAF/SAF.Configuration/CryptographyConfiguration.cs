using System;
using System.Xml;
using System.Security.Cryptography;
using System.Threading;

namespace SAF.Configuration
{
	/// <summary>
	/// CryptographyConfiguration provide teh configuration for SAF.Crytography component
	/// </summary>
	public class CryptographyConfiguration
	{
		XmlNode CryptoXml;
		internal CryptographyConfiguration(XmlNode configData)
		{
			CryptoXml = configData;
		}
		/// <summary>
		/// Retrive Xml contain the encryption/decryption information for
		/// a given profile
		/// </summary>
		/// <param name="name">profile name</param>
		/// <returns>XmlNode containing the profile information.</returns>
		public XmlNode SearchCryptoInfoByProfileName(string name)
		{
			return CryptoXml.SelectSingleNode("//Profiles/Profile[@name='" + name + "']");

		}

		/// <summary>
		/// Returns a SymmetricAlgorithm object specified in the profile
		/// </summary>
		/// <param name="profile">profile name</param>
		/// <returns>SymmetricAlgorithm object</returns>
		public SymmetricAlgorithm GetSymmetricAlgorithmProvider(string profile)
		{
			XmlNode xml = SearchCryptoInfoByProfileName(profile);
			return SymmetricAlgorithm.Create(xml.Attributes["algorithm"].Value);
		
		}

		/// <summary>
		/// Returns a AsymmetricAlgorithm object specified in the profile
		/// </summary>
		/// <param name="profile">profile name</param>
		/// <returns>AsymmetricAlgorithm object</returns>
		public AsymmetricAlgorithm GetAymmetricAlgorithmProvider(string profile)
		{
			XmlNode xml = SearchCryptoInfoByProfileName(profile);
			return AsymmetricAlgorithm.Create(xml.Attributes["algorithm"].Value);
		}

		/// <summary>
		/// Retrieve the caller's identity information based on url for the remoting calls.
		/// This allow a single application to act as different clients and carries different
		/// set of cryptography information when making calls
		/// to different urls.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public string GetIdentityByURI(string uri)
		{
			XmlNode dest = CryptoXml.SelectSingleNode("//CryptoRemotingClientSink/Identity[@uri='" + uri + "']");
			return dest.Attributes["name"].Value;

		}

		public string GetIdentity()
		{
			return Thread.CurrentPrincipal.Identity.Name;

		}
		/// <summary>
		/// Return the profile information for a specific sink type 
		/// and identity.
		/// </summary>
		/// <param name="identity"></param>
		/// <param name="cryptoType"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		public string GetProfileNameByIdentity(string identity, string cryptoType, string group)
		{
			string location;
			if (group == "ClientSink")
			{
				location = "//CryptoRemotingClientSink";
			}
			else if(group == "ServerSink")
			{
				location ="//CryptoRemotingServerSink";
			}
			else 
			{
				location = "//CryptoWSEOutputFilter";
			}

			string xpath = location + "/Identity[@name='" + identity + "']";
			XmlNode identityInfo = CryptoXml.SelectSingleNode(xpath);
			string profileName =null;
			if (cryptoType == "Encrypt")
			{
				profileName = identityInfo.SelectSingleNode("EncryptProfile").Attributes["name"].Value;
			}
			if (cryptoType == "Decrypt")
			{
				profileName = identityInfo.SelectSingleNode("DecryptProfile").Attributes["name"].Value;
			}

			return profileName;
		}

		/// <summary>
		/// check if a specific profile is for symmetric cryptography or asymmetric cryptography.
		/// </summary>
		/// <param name="name">profile name</param>
		/// <returns>boolean, true for symmatric cryptography. false for asymmatric cryptography</returns>
		public bool CheckIfSymmatric(string name)
		{
			string symmetric = CryptoXml.SelectSingleNode("//Profiles/Profile[@name='" + name + "']").Attributes["symmetric"].Value;
			return bool.Parse(symmetric);
		}

		/// <summary>
		/// Retrieve idenity information for the server where server sink is installed.
		/// it is used to help server sink to retrieve the correct cryptography profile information to 
		/// encrypt/decrypt the remoting calls.
		/// </summary>
		/// <returns></returns>
		public string GetServerSinkIndentity()
		{
			string identity =  CryptoXml.SelectSingleNode("//CryptoRemotingServerSink").Attributes["serverSinkIdentity"].Value;
			return identity;
		}

		
	}

	
}
