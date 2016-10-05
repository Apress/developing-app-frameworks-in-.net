using System;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using SAF.Configuration;
using System.Configuration;
using System.Text;
using System.Collections;
using System.Threading;

namespace SAF.Cryptography
{
	/// <summary>
	/// CryptoRemotingClientSink represents the remoting sink on the client.
	/// It enables the secure data transmitted via .NET remoting
	/// </summary>
	public class CryptoRemotingClientSink : BaseChannelSinkWithProperties,IClientChannelSink
	{
		private IClientChannelSink next;
		private Uri destination;
		private  static ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
		private  static CryptographyConfiguration cc = cm.CryptographyConfig;
		private CryptoRemotingSinkHelper helper = new CryptoRemotingSinkHelper();
		private const string sinkType = "ClientSink";

		public CryptoRemotingClientSink()
		{
		}
		public CryptoRemotingClientSink(IClientChannelSink nextSink, string url)
		{
			//set up the next sink and final destination uri
			next = nextSink;
			destination = new Uri(url);
		}

		/// <summary>
		/// interface property to retrieve the next sink
		/// </summary>
		public IClientChannelSink NextChannelSink
		{
			get { return next; }
		}
		public void AsyncProcessRequest(
			IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			throw new RemotingException("AsyncProcessRequest is not enabled yet");
		}

		public void AsyncProcessResponse(
			IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
		{
			throw new RemotingException("AsyncProcessRequest is not enabled yet");
		}


		/// <summary>
		/// ProcessMessage encrypt the data stream and send it over 
		/// to the server and decrypt the response data stream from the server
		/// </summary>
		/// <param name="msg">IMessage object</param>
		/// <param name="requestHeaders">header object for the request</param>
		/// <param name="requestStream">request stream</param>
		/// <param name="responseHeaders">output parameter containing the response header information</param>
		/// <param name="responseStream">response stream</param>
		public void ProcessMessage(
			IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, 
			out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			string identity =cc.GetIdentityByURI(destination.AbsoluteUri);
			//add the sender's identity to request header; to be retrieve on 
			//the server side
			requestHeaders["Identity"] = identity;
	
			//create the encrypted stream for the request
			Stream encryptedStream = helper.EncryptStream(identity,sinkType,requestHeaders,requestStream);

			//pass the encrypted request to next sink of the chain
			next.ProcessMessage(
				msg, requestHeaders, encryptedStream, 
				out responseHeaders, out responseStream);

			//retrieve the identity information om server side
			identity = responseHeaders["Identity"].ToString();
			//decrypt the response stream
			Stream decryptedStream = helper.DecryptStream(identity,sinkType,responseHeaders,responseStream);
			//set the output parameter responseStream to the decrypted stream
			responseStream = decryptedStream;			
		}

		public Stream GetRequestStream(System.Runtime.Remoting.Messaging.IMessage msg, System.Runtime.Remoting.Channels.ITransportHeaders headers)
		{
			//not implemented, since only formatter sink need to call this method
			return null;
		}
	}

	/// <summary>
	/// provider class for the client sink class
	/// </summary>
	public class CryptoRemotingClientSinkProvider : IClientChannelSinkProvider
	{
		private IClientChannelSinkProvider next;
		public CryptoRemotingClientSinkProvider(IDictionary properties, ICollection providerData) 
		{
		}
		public IClientChannelSinkProvider Next
		{
			get { return next; }
			set { next = value; }
		}

		/// <summary>
		/// factory method that create the concrete corresponding client sink object
		/// </summary>
		/// <param name="channel">channel object</param>
		/// <param name="url">destination's uri</param>
		/// <param name="remoteChannelData"></param>
		/// <returns>newly created CyrptoRemotingClientSink object </returns>
		public IClientChannelSink CreateSink(System.Runtime.Remoting.Channels.IChannelSender channel, string url, object remoteChannelData)
		{
			
			IClientChannelSink nextSink;
			//create the next sink that is located after the new CryptoRemotingClientSink object
			nextSink = next.CreateSink(channel, url, remoteChannelData);
			//create and return the new CyrptoRemotingClientSink object
			return new CryptoRemotingClientSink(nextSink,url);
		}


	}


}
