using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.GridFS;
using homefinderpro.Models;
using homefinderpro.LandlordModels;

namespace homefinderpro.LandlordViewModels
{
    public class Landlord : IUser
    {
        public ObjectId Id { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public byte[] ProfilePicture { get; set; }
    }
}
