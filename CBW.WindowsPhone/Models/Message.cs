using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBW.WindowsPhone
{
    public enum MessageSide
    {
        User,
        Cloud
    }

    public class Message
    {
        public string Content { get; set; }
        public DateTime Time { get; set; }
        public MessageSide Side { get; set; }

        public Message()
        {
        }

        public Message(string content, MessageSide side)
        {
            this.Content = content;
            this.Side = side;
            this.Time = DateTime.Now;
        }
    }

    public class MessageCollection : ObservableCollection<Message>
    {
    }
}
