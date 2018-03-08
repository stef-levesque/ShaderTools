//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using ShaderTools.LanguageServer.Protocol.Utilities;

namespace ShaderTools.LanguageServer.Protocol.MessageProtocol.Channel
{
    public class TcpSocketServerChannel : ChannelBase
    {
        private TcpClient tcpClient;
        private TcpListener tcpListener;
        private NetworkStream networkStream;
        private IMessageSerializer messageSerializer;

        public TcpSocketServerChannel(int portNumber)
        {
            this.tcpListener = new TcpListener(IPAddress.Loopback, portNumber);
            this.tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            this.tcpListener.Start();
        }

        public override async Task WaitForConnection()
        {
            this.tcpClient = await this.tcpListener.AcceptTcpClientAsync();
            this.networkStream = this.tcpClient.GetStream();

            this.MessageReader =
                new MessageReader(
                    this.networkStream,
                    this.messageSerializer);

            this.MessageWriter =
                new MessageWriter(
                    this.networkStream,
                    this.messageSerializer);

            this.IsConnected = true;
        }

        protected override void Initialize(IMessageSerializer messageSerializer)
        {
            this.messageSerializer = messageSerializer;
        }

        protected override void Shutdown()
        {
            if (this.tcpListener != null)
            {
                this.networkStream.Dispose();
                this.tcpListener.Stop();
                this.tcpListener = null;

                Logger.Write(LogLevel.Verbose, "TCP listener has been stopped");
            }

            if (this.tcpClient != null)
            {
                this.tcpClient.Dispose();
                this.tcpClient = null;

                Logger.Write(LogLevel.Verbose, "TCP client has been closed");
            }
        }
    }
}