using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Tcp;
using Net.Messaging;
using System.Net;

namespace Tcp.Client2
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var connetion = TcpConnection.Connect(IPAddress.Loopback, 8080))
            {
                connetion.IncomingMessage += OnIncomingMessage;
                connetion.Receive();
            }
        }

        private static void OnIncomingMessage(object sender, IncomingMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
