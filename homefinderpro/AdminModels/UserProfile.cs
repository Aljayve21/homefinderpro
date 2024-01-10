using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homefinderpro.Models;

namespace homefinderpro.AdminModels
{
    [BsonIgnoreExtraElements]
    public class UserProfile : IUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
