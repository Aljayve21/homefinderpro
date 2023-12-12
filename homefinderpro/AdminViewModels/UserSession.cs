using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.AdminViewModels
{
    public class UserSession
    {
        private static UserSession _instance;

        public string Username { get; private set; }
        public string Fullname { get; private set; }
        public string Role { get; private set; }

        private UserSession() { }

        public static UserSession Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new UserSession();
                }
                return _instance;
            }
        }

        public void SetUser(string username, string fullname, string role)
        {
            Username = username;
            Fullname = fullname;
            Role = role;
        }

        public void ClearUser()
        {
            Username = null;
            Fullname = null;
        }

        public bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(Username);
        }
    }
}
