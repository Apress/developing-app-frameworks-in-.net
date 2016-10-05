using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using SAF.Configuration;
using System.Configuration;
using System.Text;
using System.Collections;

namespace SAF.Cryptography
{
	/// <summary>
	/// provide functions that provide an easier access the SAF.Crytography.
	/// </summary>
	public class CryptoRemotingSinkHelper
	{
		private static ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
		private static CryptographyConfiguration cc = cm.CryptographyConfig;
		public CryptoRemotingSinkHelper()
		{
			
		}

		/// <summary>
		/// Encrypt the stream
		/// </summary>
		/// <param name="identity">identity name</param>
		/// <param name="sinkType">type of the client, either ClientSink or ServerSink</param>
		/// <param name="transportHeaders">Transport header object</param>
		/// <param name="targetStream">stream to be encryted</param>
		/// <returns></returns>
		public Stream EncryptStream(string identity, string sinkType,ITransportHeaders transportHeaders, Stream targetStream)
		{
			byte[] key;
			byte[] iv;
			byte[] signature;
			Stream encryptedStream = null;

			string profileName = cc.GetProfileNameByIdentity(identity,"Encrypt",sinkType);
			//perform the data encryption on request
			if (cc.CheckIfSymmatric(profileName))
			{
				//put information into the header to be sent over with the remoting call
				transportHeaders["Identity"] =  identity;
				encryptedStream = Encryption.Encrypt(targetStream,profileName);
			}
			else
			{
				//put information into the header to be sent over with the remoting call
				encryptedStream = Encryption.Encrypt(targetStream,profileName,out key,out iv,out signature);
				transportHeaders["Identity"] =  identity;
				transportHeaders["key"] = ConvertBytesToString(key);
				transportHeaders["iv"] =ConvertBytesToString(iv);
				transportHeaders["signature"] = ConvertBytesToString(signature);
				
			}
			return encryptedStream;
		}



		/// <summary>
		/// A helper method of to decrypt stream for a specific
		/// remoting call.
		/// </summary>
		/// <param name="identity">call identity</param>
		/// <param name="sinkType">sink type, either server sink or client sink</param>
		/// <param name="transportHeaders">ITransportHeaders object</param>
		/// <param name="targetStream">stream to be decrypted.</param>
		/// <returns>decrypted stream</returns>
		public Stream DecryptStream(string identity, string sinkType, ITransportHeaders transportHeaders, Stream targetStream)
		{
			byte[] key;
			byte[] iv;
			byte[] signature;
			Stream decryptedStream = null;

			string profileName = cc.GetProfileNameByIdentity(identity,"Decrypt",sinkType);
			//perform the data encryption on request
			if (cc.CheckIfSymmatric(profileName))
			{
				//decrypt the stream.
				decryptedStream = Decryption.Decrypt(targetStream,profileName);
			}
			else
			{
				//retrieve addtional information from the transport headers.
				key = ConvertStringToBytes(transportHeaders["key"].ToString());
				iv = ConvertStringToBytes(transportHeaders["iv"].ToString());
				signature =ConvertStringToBytes(transportHeaders["signature"].ToString());	
				//decrypt the stream
				decryptedStream = Decryption.Decrypt(targetStream,profileName,key,iv,signature);
			}
			return decryptedStream;
		}

		/// <summary>
		/// Convert the byte array to comma delimited string.
		/// Because .NET framework transport header doesn't handle the encoding properly as in .NET 1.1,
		/// we need to conver the byte[] to a string such as "142,24,...". Simply calling GetString(byte[])
		/// would result corrupted information when reading from transport header on the server side.
		/// </summary>
		/// <param name="bytes">byte[]</param>
		/// <returns>comma delimited string</returns>
		private string ConvertBytesToString(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder();
			foreach (byte b in bytes)
			{
				sb.Append(b.ToString() + ",");
			}
			sb.Remove(sb.Length - 1,1);
			return sb.ToString();
		}

		/// <summary>
		/// Convert the comma delimited string into bypte[]. 
		/// </summary>
		/// <param name="text">comma delimited string</param>
		/// <returns>byte[]</returns>
		private byte[] ConvertStringToBytes(string text)
		{
			string[] strings = text.Split(',');
			byte[] bytes = new byte[strings.Length];
			for(int i =0; i < strings.Length; i++)
			{
				bytes[i] = (byte)Int32.Parse(strings[i]);
			}

			return bytes;							 
		}
	}
}
