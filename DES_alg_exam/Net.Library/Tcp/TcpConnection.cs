using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Net.Tcp
{
    public class IncomingMessageEventArgs : EventArgs
    {
        public IMessage _message { get; internal set; }

        public TCPConnection _connection { get; internal set; }
    }

    public class TCPConnection : IDisposable
    {
        public TcpClient _client { get; }

        public event EventHandler<IncomingMessageEventArgs> _incomingMessage;

        public TCPConnection(TcpClient client)
        {
            _client = client;
        }

        public void Send(IMessage message)
        {
            _client.Send(message);
        }

        // if there is message - receive it and initiate event
        public void Receive()
        {
            if (_client.GetStream().DataAvailable)
            {
                IMessage message = _client.Receive();
                var args = new IncomingMessageEventArgs
                {
                    _message = message,
                    _connection = this
                };
                _incomingMessage?.Invoke(this, args);
            }
        }

        public void Dispose()
        {
            if (_client.Connected)
            {
                _client.Close();
            }
        }

        public static TCPConnection Connect(IPAddress address, int port)
        {
            TcpClient client = new TcpClient();
            client.Connect(address, port);
            return new TCPConnection(client);
        }
    }
}
