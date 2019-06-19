using Net.Tcp;
using System;

namespace Net.Messaging
{
    [Serializable]
    public class TextMessage : IMessage
    {
        public string Text { get; }

        public TextMessage(string text)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
