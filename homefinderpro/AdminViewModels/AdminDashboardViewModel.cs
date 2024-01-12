using homefinderpro.ChatModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.AdminViewModels
{
    public class AdminDashboardViewModel
    {
        private LiveChat _liveChat;

        public ObservableCollection<ChatMessage> AdminChatMessages { get; set; } = new ObservableCollection<ChatMessage>();

        public AdminDashboardViewModel(LiveChat liveChat)
        {
            _liveChat = liveChat;
        }

        public void LoadAdminChatMessages()
        {
            string adminId = UserSession.Instance.Username;

            // Load chat messages between admin and users
            var adminChatMessages = _liveChat.GetChatMessages(adminId, "admin");

            if (adminChatMessages != null)
            {
                AdminChatMessages.Clear();
                foreach (var chatMessageModel in adminChatMessages)
                {
                    AdminChatMessages.Add(chatMessageModel);
                }
            }
        }

        public bool SendMessageToAdmin(string senderId, string message)
        {
            // Send a message to the admin
            return _liveChat.SendMessage(senderId, "admin", message);
        }
    }
}
