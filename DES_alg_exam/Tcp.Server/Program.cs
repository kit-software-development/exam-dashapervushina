using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Tcp;
using Net.Messaging;
using System.Net;

namespace Tcp.Server2
{
    class Program
    {
        private const int quantityOfRounds = 16; //количество раундов
        private const int shiftKey = 2; //сдвиг ключа 
        string[] Blocks; //сами блоки в двоичном формате

        void Main(string[] args)
        {
            // listen for incoming connection
            var listener = new TCPConnectionListener(IPAddress.Any, 8080);
            listener._incomingConnection += OnIncomingConnection;
            listener.Start();
            Console.WriteLine("Server started. Press Enter to exit.\n");

            // wait for 'Enter' to stop listener
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            while (keyInfo.Key != ConsoleKey.Enter)
                keyInfo = Console.ReadKey();

            listener.Stop();
        }

        private void OnIncomingConnection(object sender, IncomigConnectionEventArgs e)
        {
            e._connection._incomingMessage += OnIncomingMessage;
            // workaround for response that has not come yet
            System.Threading.Thread.Sleep(1000);
            e._connection.Receive();
        }

        // получаем зашифрованную строку и ключ
        private static Tuple<string, string> GetMessageAndKey(string data)
        {
            int i;
            string message = "", key = "";

            for (i = data.Length - 1; i >= 0; i--)
            {
                if (data[i] != '~') key += data[i];
                else break;
            }

            for (int j = 0; j < i; j++) message += data[j];

            return Tuple.Create(message, key);
        }

        private void OnIncomingMessage(object sender, IncomingMessageEventArgs e)
        {
            // encrypted message and client id
            var mess = GetMessageAndKey(e._message.ToString());
            // decrypt message using private key
            string decryptedMessage;
            
            decryptedMessage = Decrypt(mess.Item1, mess.Item2);
            Console.WriteLine("Key " + mess.Item2 + "> " + decryptedMessage);
            e._connection.Dispose();
        }

        public string Decrypt(string encryptedMessage, string key)
        {
            for (int j = 0; j < quantityOfRounds; j++)
            {
                for (int i = 0; i < Blocks.Length; i++)
                    Blocks[i] = DecodeDES_One_Round(Blocks[i], key);

                key = KeyToPrevRound(key);
            }

            key = KeyToNextRound(key);

            //textBoxEncodeKeyWord.Text = StringFromBinaryToNormalFormat(key);

            string result = "";

            for (int i = 0; i < Blocks.Length; i++)
                result += Blocks[i];

            return result;
        }

        //шифрующая функция f. в данном случае это XOR
        private string f(string s1, string s2)
        {
            return XOR(s1, s2);
        }

        //расшифровка DES один раунд
        private string DecodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (XOR(f(L, key), R) + L);
        }

        //вычисление ключа для следующего раунда шифрования. циклический сдвиг >> 2
        private string KeyToNextRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }

            return key;
        }

        //вычисление ключа для следующего раунда расшифровки. циклический сдвиг << 2
        private string KeyToPrevRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key + key[0];
                key = key.Remove(0, 1);
            }

            return key;
        }

        //XOR двух строк с двоичными данными
        private string XOR(string s1, string s2)
        {
            string result = "";

            for (int i = 0; i < s1.Length; i++)
            {
                bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
                bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

                if (a ^ b)
                    result += "1";
                else
                    result += "0";
            }
            return result;
        }

    }
}
