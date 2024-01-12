using homefinderpro.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.ChatModels
{
    public class LiveChat
    {
        private readonly DBConnection _dbConnection;
        public LiveChat(DBConnection dBConnection)
        {
            _dbConnection = dBConnection;
        }

        public bool SendMessage(string senderId, string receiverId, string message)
        {
            try
            {
                var chatMessageModel = new ChatMessageModel(message)
                {
                    // Initialize properties of ChatMessageModel if needed
                };
                var chatMessage = new ChatMessage(chatMessageModel)
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = message,
                    Timestamp = DateTime.Now
                };

                var database = _dbConnection.GetDatabase();
                var chatCollection = database.GetCollection<ChatMessage>("livechat");

                chatCollection.InsertOne(chatMessage);

                return true;
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error sending chat messages: {ex.Message}");
                return false;
            }
        }



        public List<ChatMessage> GetChatMessages(string  userId1, string userId2)
        {
            try
            {
                var database = _dbConnection.GetDatabase();
                var chatCollection = database.GetCollection<ChatMessage>("livechat");

                var filter = Builders<ChatMessage>.Filter.Or(
                    Builders<ChatMessage>.Filter.And(
                        Builders<ChatMessage>.Filter.Eq("SenderId", userId1),
                        Builders<ChatMessage>.Filter.Eq("ReceiverId", userId2)
                    ),
                    Builders<ChatMessage>.Filter.And(
                        Builders<ChatMessage>.Filter.Eq("SenderId", userId2),
                        Builders<ChatMessage>.Filter.Eq("ReceiverId", userId1)
                    )
                );

                var chatMessages = chatCollection.Find(filter).SortBy(x => x.Timestamp).ToList();

                return chatMessages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting chat messages: {ex.Message}");
                return null;
            }
        }

        public bool SendMessageToUser(string senderId, string receiverId, string message)
        {
            return SendMessage(senderId, receiverId, message);
        }

        public bool SendMessageToLandlord(string senderId, string landlordId, string message)
        {
            return SendMessage(senderId, landlordId, message);
        }

        public bool SendMessageToCustomer(string senderId, string customerId, string message)
        {
            return SendMessage(senderId, customerId, message);
        }

        public List<ChatMessage> GetChatMessagesForUser(string userId1, string userId2)
        {
            return GetChatMessages(userId1, userId2);
        }

        public List<ChatMessage> GetChatMessagesForLandlord(string landlordId, string userId)
        {
            return GetChatMessages(landlordId, userId);
        }

        public List<ChatMessage> GetChatMessagesForCustomer(string customerId, string userId)
        {
            return GetChatMessages(customerId, userId);
        }
    }
 }

