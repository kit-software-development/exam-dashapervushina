using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Net.Sockets;

namespace Net.Tcp
{
    static class Extensions
    {
        private static BinaryFormatter _bFormatter = new BinaryFormatter();

        public static byte[] ToBytes(this IMessage message)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                _bFormatter.Serialize(memory, message);
                memory.Flush();
                return memory.ToArray();
            }
        }

        public static IMessage FromBytes(byte[] data)
        {
            using (MemoryStream memory = new MemoryStream(data))
            {
                return (IMessage)_bFormatter.Deserialize(memory);
            }
        }

        static void Write(this NetworkStream stream, IMessage message)
        {
            byte[] bytes = message.ToBytes();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }

        public static void Send(this TcpClient client, IMessage message)
        {
            client.GetStream().Write(message);
        }

        static IMessage Read(this NetworkStream stream)
        {
            return (IMessage)_bFormatter.Deserialize(stream);
        }

        public static IMessage Receive(this TcpClient client)
        {
            return client.GetStream().Read();
        }
    }
}
