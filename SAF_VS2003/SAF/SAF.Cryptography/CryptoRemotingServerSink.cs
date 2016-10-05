using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.IO;
using SAF.Configuration;
using System.Configuration;
using System.Collections;
using System.Threading;

namespace SAF.Cryptography
{
	/// <summary>
	/// CryptoRemotingServerSink represents the remoting sink on the server.
	/// It enables the secure data transmitted via .NET remoting
	/// </summary>
	public class CryptoRemotingServerSink : BaseChannelSinkWithProperties, IServerChannelSink
	{
		private IServerChannelSink next;
		private  static ConfigurationManager cm = (ConfigurationManager)ConfigurationSettings.GetConfig("Framework");
		private  static CryptographyConfiguration cc = cm.CryptographyConfig;
		private CryptoRemotingSinkHelper helper = new CryptoRemotingSinkHelper();
		private const string sinkType = "ServerSink";

		public CryptoRemotingServerSink()
		{
		}


		/// <summary>
		/// ProcessMessage decrypts the data stream and forward it the target 
		/// server object and encrypts the return value
		/// </summary>
		/// <param name="sinkStack">channel sink object</param>
		/// <param name="requestMsg">IMessage object</param>
		/// <param name="transportHeaders">transport header object</param>
		/// <param name="targetStream">requrest stream</param>
		/// <param name="responseMsg">output parameters containing response message</param>
		/// <param name="responseHeaders">output parameter containing the response header</param>
		/// <param name="responseStream">output parameter containing the response stream</param>
		/// <returns></returns>
		public ServerProcessing ProcessMessage(
			IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders transportHeaders, Stream targetStream, 
			out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
		{
			
			//extract the identity information from the header
			string identity = transportHeaders["Identity"].ToString();
			LocalDataStoreSlot dataSlot = null;
			//create an thread data slot
			dataSlot = Thread.GetNamedDataSlot("Identity");

			//add the identity informatioin to the data slot on the thread so that
			//server object can determine who made the request.
			Thread.SetData(dataSlot,identity);
			// Push this onto the sink stack
			sinkStack.Push(this, null);

			//decrypt the request stream
			Stream decryptedStream = helper.DecryptStream(identity,sinkType,transportHeaders,targetStream);
			ServerProcessing processingResult;
		
			//pass the decrypted request to next sink of the chain
			processingResult = next.ProcessMessage(
				sinkStack, requestMsg, transportHeaders, decryptedStream,
				out responseMsg, out responseHeaders, out responseStream);
			
			
			//encrypte the response stream.
			Stream encryptedStream = helper.EncryptStream(identity,sinkType,responseHeaders,responseStream);
			string serverIdentity = cc.GetServerSinkIndentity();
			responseHeaders["Identity"] = serverIdentity;
			responseStream  = encryptedStream;
						
			return processingResult;
		}

		public void AsyncProcessResponse(
			IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers, Stream stream)
		{
			throw new RemotingException("AsyncProcessRequest is not enabled yet");
		}
		public CryptoRemotingServerSink(IServerChannelSink nextSink)
		{
			next = nextSink;
		}

		public Stream GetResponseStream(
			IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
		{
			//not implemented, since only formatter sink need to call this method
			return null;
		}
		public IServerChannelSink NextChannelSink
		{
			get { return next; }
		}
	}

	/// <summary>
	/// provider class for the server sink class
	/// </summary>
	public class CryptoRemotingServerSinkProvider : IServerChannelSinkProvider
	{
		private IServerChannelSinkProvider next;
		public CryptoRemotingServerSinkProvider(IDictionary properties, ICollection providerData)
		{
		}


		public IServerChannelSinkProvider Next
		{
			get { return next; }
			set { next = value; }
		}
		public void GetChannelData(IChannelDataStore channelData)
		{
		}
		/// <summary>
		/// factory method that create the concrete corresponding server sink object
		/// </summary>
		/// <param name="channel">The channel for which to create the channel sink chain. </param>
		/// <returns>newly created CryptoRemotingServerSink object</returns>
		public IServerChannelSink CreateSink(IChannelReceiver channel)
		{
			IServerChannelSink nextSink;
			nextSink = next.CreateSink(channel);
			return new CryptoRemotingServerSink(nextSink);
		}
	}


}
