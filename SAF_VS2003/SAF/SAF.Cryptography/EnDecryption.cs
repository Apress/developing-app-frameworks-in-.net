using System;
using SAF.Configuration;
using System.Configuration;
using System.Xml;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Microsoft.Web.Services.Security.X509;

namespace SAF.Cryptography
{
	/// <summary>
	/// It consists methods to encrypt string and stream.
	/// </summary>
	public class Encryption
	{
		public Encryption()
		{
		}
		/// <summary>
		/// encrypt the string data asymmetrically using the encryption information
		/// of specific security profile in the configuration file
		/// </summary>
		/// <param name="data">data string</param>
		/// <param name="profile">security profile containig the key information</param>
		/// <param name="key">output parameter containing the generated secret key </param>
		/// <param name="iv">output parameter containing the generated iv key</param>
		/// <param name="signature">output parameter containing the generated digital dignature</param>
		/// <returns>dencrypted string</returns>
		public static string Encrypt(string data, string profile, out byte[] key, out byte[] iv, out byte[] signature )
		{
			//convert the string into stream
			MemoryStream original = new MemoryStream(Encoding.Default.GetBytes(data));
			//encrypte the stream
			Stream encryptedStream = Encryption.Encrypt(original, profile, out key, out iv, out signature);
			//convert the encrypted into back to string
			byte[] encryptedData = new byte[encryptedStream.Length];
			encryptedStream.Read(encryptedData,0,encryptedData.Length);
			return Encoding.Default.GetString(encryptedData);

		}
		/// <summary>
		/// encrypt the stream asymmetrically using the encryption information
		/// of specific security profile in the configuration file
		/// </summary>
		/// <param name="data">stream data</param>
		/// <param name="profile">security profile name</param>
		/// <param name="key">output parameter for generated secret key</param>
		/// <param name="iv">output parameter for generated iv</param>
		/// <param name="signature">out parameters for the digital signature</param>
		/// <returns>stream data</returns>
		public static Stream Encrypt(Stream data, string profile, out byte[] key, out byte[] iv, out byte[] signature)
		{
			SAF.Configuration.ConfigurationManager cm = (ConfigurationManager)System.Configuration.ConfigurationSettings.GetConfig("Framework");
			CryptographyConfiguration cc = cm.CryptographyConfig;
			//retrieve the security profile information from the configuration file
			XmlNode cryptoInfo = cc.SearchCryptoInfoByProfileName(profile);
			bool symmetric  = Boolean.Parse(cryptoInfo.Attributes["symmetric"].Value);

			ICryptoTransform encryptor =null;
			RSACryptoServiceProvider provider = null;

			if (symmetric != false)
			{
				throw new System.Exception("This method id not intended for symmetric  encryption");
			}

			provider = (RSACryptoServiceProvider)cc.GetAymmetricAlgorithmProvider(profile);
			//retireve the sneder and receiver's certification information for encryption
			string senderCert = cryptoInfo.SelectSingleNode("SenderCertificate").InnerText;
			string sendCertStore = cryptoInfo.SelectSingleNode("SenderCertificate").Attributes["store"].Value;
			string receiverCert = cryptoInfo.SelectSingleNode("ReceiverCertificate").InnerText;
			string receiverCertStore =cryptoInfo.SelectSingleNode("ReceiverCertificate").Attributes["store"].Value;
			string symmatricAlgorithm = cryptoInfo.SelectSingleNode("SymmatricAlgorithm").InnerText;
			
			//obtain the X509 certificate object for the sender and receiver
			X509Certificate senderCertificate = Certificate.SearchCertificateBySubjectName(sendCertStore,senderCert);
			X509Certificate receiverCertificate = Certificate.SearchCertificateBySubjectName(receiverCertStore,receiverCert);

			//receive the sender's private key and receiver's public key for encryption
			RSAParameters sender_privateKey = senderCertificate.Key.ExportParameters(true);
			RSAParameters receiver_publicKey = receiverCertificate.PublicKey.ExportParameters(false);
			
			SymmetricAlgorithm symmProvider = SymmetricAlgorithm.Create(symmatricAlgorithm);
			
			encryptor = symmProvider.CreateEncryptor();

			CryptoStream encStream = new CryptoStream(data, encryptor, CryptoStreamMode.Read);
			MemoryStream encrypted  = new MemoryStream();
			byte[] buffer = new byte[1024];
			int count = 0;
			while ((count = encStream.Read(buffer,0,1024)) > 0)
			{
				encrypted.Write(buffer,0,count);
			}
			//encrypt the screte key, iv key using receiver's public key
			//that are used to decrypt the data
			provider.ImportParameters(receiver_publicKey);
			
			key =  provider.Encrypt(symmProvider.Key,false);
			iv = provider.Encrypt(symmProvider.IV,false);

			//sign the data with sender's private key
			provider.ImportParameters(sender_privateKey);
			signature = provider.SignData(encrypted.ToArray(),new SHA1CryptoServiceProvider());
			encrypted.Position = 0;
			return (Stream)encrypted;
		}

		/// <summary>
		/// encrypt the stream symmetrically using the security profile
		/// information stored in the configuration file
		/// </summary>
		/// <param name="data">stream data</param>
		/// <param name="profile">profile name</param>
		/// <returns>stream data</returns>
		public static Stream Encrypt(Stream data, string profile)
		{
			SAF.Configuration.ConfigurationManager cm = (ConfigurationManager)System.Configuration.ConfigurationSettings.GetConfig("Framework");
			CryptographyConfiguration cc = cm.CryptographyConfig;
			//retrieve security profile information
			XmlNode cryptoInfo = cc.SearchCryptoInfoByProfileName(profile);
			bool symmetric  = Boolean.Parse(cryptoInfo.Attributes["symmetric"].Value);

			ICryptoTransform encryptor =null;
			SymmetricAlgorithm provider = null;

			if (symmetric != true)
			{
				throw new System.Exception("This method id not intended for asymmetric  encryption");
			}

			//retrive the secret key and iv information
			provider = cc.GetSymmetricAlgorithmProvider(profile);
			string key = cryptoInfo.SelectSingleNode("SecretKey").InnerText;
			string iv = cryptoInfo.SelectSingleNode("IV").InnerText;

			provider.Key = Encoding.Default.GetBytes(key);
			provider.IV = Encoding.Default.GetBytes(iv);

			encryptor = provider.CreateEncryptor();
			MemoryStream encrypted  = new MemoryStream();
			//encrypt the stream symmetrically
			CryptoStream encStream = new CryptoStream(encrypted, encryptor, CryptoStreamMode.Write);
			
			byte[] buffer = new byte[1024];
			int count = 0;
			while ((count = data.Read(buffer,0,1024)) > 0)
			{
				encStream.Write(buffer,0,count);
			}
			encStream.FlushFinalBlock();
			encrypted.Position = 0 ;
			return (Stream)encrypted;
		}

		/// <summary>
		/// encrypt the string data symmetrically using the security profile
		/// information stored in the configuration file
		/// </summary>
		/// <param name="data">string data</param>
		/// <param name="profile">security profile name</param>
		/// <returns>encrypted string</returns>
		public static string Encrypt(string data, string profile)
		{
			//convert the string to stream
			MemoryStream original = new MemoryStream(Encoding.Default.GetBytes(data));
			Stream encryptedStream = Encryption.Encrypt(original,profile);
			byte[] encryptedData = new Byte[encryptedStream.Length];
			encryptedStream.Read(encryptedData,0,encryptedData.Length);
			//convert the encrytped stream to string
			return Encoding.Default.GetString(encryptedData);
		}
	}

	/// <summary>
	///Contains functionalities to decrypt data
	/// </summary>
	public class Decryption
	{
		/// <summary>
		/// decrypt the string data asymmetrically using the security profile
		/// information stored in the configuration file
		/// </summary>
		/// <param name="data">encrypted string data</param>
		/// <param name="profile">security profile name</param>
		/// <param name="key">encrypted secret key</param>
		/// <param name="iv">genrated iv</param>
		/// <param name="signature">generated signature</param>
		/// <returns>Decrypted string</returns>
		public static string Decrypt(string data, string profile,  byte[] key,  byte[] iv,  byte[] signature )
		{
			//convert the string to stream
			MemoryStream original = new MemoryStream(Encoding.Default.GetBytes(data));
			Stream decryptedStream = Decryption.Decrypt(original, profile,  key,  iv,  signature);
			byte[] decryptedData = new Byte[decryptedStream.Length];
			decryptedStream.Read(decryptedData,0,decryptedData.Length);
			//convert the decrypted stream to string
			return Encoding.Default.GetString(decryptedData);

		}

		/// <summary>
		/// decrypt the stream data asymmetrically using the security profile
		/// information stored in the configuration file
		/// </summary>
		/// <param name="data">encrypted stream data</param>
		/// <param name="profile">security profile name</param>
		/// <param name="key">generated key</param>
		/// <param name="iv">generated iv</param>
		/// <param name="signature">generated signature</param>
		/// <returns>decrypted stream</returns>
		public static Stream Decrypt(Stream data, string profile,  byte[] key,  byte[] iv,  byte[] signature)
		{
			SAF.Configuration.ConfigurationManager cm = (ConfigurationManager)System.Configuration.ConfigurationSettings.GetConfig("Framework");
			CryptographyConfiguration cc = cm.CryptographyConfig;
			//retrieve the security profile information
			XmlNode cryptoInfo = cc.SearchCryptoInfoByProfileName(profile);
			bool symmetric  = Boolean.Parse(cryptoInfo.Attributes["symmetric"].Value);
			ICryptoTransform decryptor =null;
			RSACryptoServiceProvider provider = null;

			if (symmetric != false)
			{
				throw new System.Exception("This method id not intended for symmetric  encryption");
			}

			provider = (RSACryptoServiceProvider)cc.GetAymmetricAlgorithmProvider(profile);
			//retrieve the sender and receiver's certification information for decryption
			string senderCert = cryptoInfo.SelectSingleNode("SenderCertificate").InnerText;
			string sendCertStore = cryptoInfo.SelectSingleNode("SenderCertificate").Attributes["store"].Value;
			string receiverCert = cryptoInfo.SelectSingleNode("ReceiverCertificate").InnerText;
			string receiverCertStore =cryptoInfo.SelectSingleNode("ReceiverCertificate").Attributes["store"].Value;
			string symmatricAlgorithm = cryptoInfo.SelectSingleNode("SymmatricAlgorithm").InnerText;
			
			//obtain X509 certification object
			X509Certificate senderCertificate = Certificate.SearchCertificateBySubjectName(sendCertStore,senderCert);
			X509Certificate receiverCertificate = Certificate.SearchCertificateBySubjectName(receiverCertStore,receiverCert);

			//retrieve the sender's private key and receiver's public
			RSAParameters sender_privateKey = senderCertificate.Key.ExportParameters(true);
			RSAParameters receiver_publicKey = receiverCertificate.PublicKey.ExportParameters(false);

			//import the public key information to verify the data
			provider.ImportParameters(receiver_publicKey);
	
			MemoryStream ms= new MemoryStream();
			byte[] buffer = new Byte[1024];
			int count =0;
			while ((count =data.Read(buffer,0,buffer.Length)) > 0)
			{
				ms.Write(buffer,0,count);
			}

			byte[] encryptedData = ms.ToArray();
			//data.Position = 0 ;
			//data.Read(encryptedData,0,encryptedData.Length);
			//verify if the data has been tempered with	
			bool v = provider.VerifyData(encryptedData,new SHA1CryptoServiceProvider(),signature);
			if (v == false)
			{
				throw new CryptographicException();
			}
			//import the private key information to decrypt data
			provider.ImportParameters(sender_privateKey);
			//decrypt the secret key and iv
			byte[] decryptedkey =  provider.Decrypt(key,false);
			byte[] decryptediv = provider.Decrypt(iv,false);
			
			SymmetricAlgorithm symmProvider = SymmetricAlgorithm.Create(symmatricAlgorithm);
			symmProvider.Key = decryptedkey;
			symmProvider.IV = decryptediv;
			decryptor = symmProvider.CreateDecryptor();
			ms.Position = 0;
			//decrypt the stream			
			CryptoStream decStream = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
			MemoryStream decrypted  = new MemoryStream();
			count = 0;
			while ((count = decStream.Read(buffer,0,buffer.Length)) != 0)
			{
				decrypted.Write(buffer,0,count);
			}
			decrypted.Position = 0 ;
			return (Stream)decrypted;
		}

		/// <summary>
		/// decrypt the stream data symmetrically using the security profile
		/// information stored in the configuration file
		/// </summary>
		/// <param name="data">stream data</param>
		/// <param name="profile">security profile name</param>
		/// <returns>decrypted stream</returns>
		public static Stream Decrypt(Stream data, string profile)
		{
			SAF.Configuration.ConfigurationManager cm = (ConfigurationManager)System.Configuration.ConfigurationSettings.GetConfig("Framework");
			CryptographyConfiguration cc = cm.CryptographyConfig;
			//retrieve the security profile information for configuration file
			XmlNode cryptoInfo = cc.SearchCryptoInfoByProfileName(profile);
			bool symmetric  = Boolean.Parse(cryptoInfo.Attributes["symmetric"].Value);

			ICryptoTransform decryptor =null;
			SymmetricAlgorithm provider = null;

			if (symmetric != true)
			{
				throw new System.Exception("This method id not intended for asymmetric  encryption");
			}
			//retrieve the secret key and iv from the configuration file			
			provider = cc.GetSymmetricAlgorithmProvider(profile);
			string key = cryptoInfo.SelectSingleNode("SecretKey").InnerText;
			string iv = cryptoInfo.SelectSingleNode("IV").InnerText;

			provider.Key = Encoding.Default.GetBytes(key);
			provider.IV = Encoding.Default.GetBytes(iv);
			decryptor = provider.CreateDecryptor();
			//decrypt the stream 
			CryptoStream decStream = new CryptoStream(data, decryptor, CryptoStreamMode.Read);
			MemoryStream decrypted  = new MemoryStream();
			byte[] buffer = new byte[2048];
			int count = 0;
		
			while ((count = decStream.Read(buffer,0,buffer.Length)) != 0)
			{
				decrypted.Write(buffer,0,count);
			}
			
			decrypted.Position = 0 ;
			return (Stream)decrypted;
		}

		/// <summary>
		/// decrypt the string data symmetrically using the security profile
		/// information stored in the configuration file
		/// </summary>
		/// <param name="data">encrypted string data</param>
		/// <param name="profile">security profile name</param>
		/// <returns>decrypted string</returns>
		public static string Decrypt(string data, string profile)
		{
			//convert the string to stream
			MemoryStream original = new MemoryStream(Encoding.Default.GetBytes(data));
			Stream decryptedStream = Decryption.Decrypt(original,profile);
			byte[] decryptedData = new Byte[decryptedStream.Length];
			decryptedStream.Read(decryptedData,0,decryptedData.Length);
			//convert the stream to  string
			return Encoding.Default.GetString(decryptedData);
			
		}

	}


	


}
