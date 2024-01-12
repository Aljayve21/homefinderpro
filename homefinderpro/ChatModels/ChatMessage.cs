using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.ChatModels
{
    public class ChatMessage
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage(ChatMessageModel chatMessage)
        {
            SenderId = chatMessage.SenderId;
            ReceiverId = chatMessage.ReceiverId;
            Message = chatMessage.Message;
            Timestamp = chatMessage.Timestamp;
        }
    }
}
