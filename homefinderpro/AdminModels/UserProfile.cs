using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.AdminModels
{
    public class UserProfile
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Role { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
