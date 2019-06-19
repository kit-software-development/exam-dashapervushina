using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace Net.Tcp
{
    public class IncomigConnectionEventArgs : EventArgs
    {
        public TCPConnection _connection { get; internal set; }
    }

    public class TCPConnectionListener
    {
        private Thread _thread;

        private TcpListener _listener;

        public event EventHandler<IncomigConnectionEventArgs> _incomingConnection;

        public TCPConnectionListener(IPAddress address, int port)
        {
            _listener = new TcpListener(address, port);
            _thread = new Thread(Listen);
            _thread.IsBackground = true;
        }

        private void Listen()
        {
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                TCPConnection connection = new TCPConnection(client);
                var args = new IncomigConnectionEventArgs
                {
                    _connection = connection
                };
                _incomingConnection?.Invoke(this, args);
            }
        }

        public void Start()
        {
            _listener.Start();
            _thread.Start();
        }

        public void Stop()
        {
            if (_thread.IsAlive)
            {
                _thread.Interrupt();
            }
            _listener.Stop();
        }
    }
}
