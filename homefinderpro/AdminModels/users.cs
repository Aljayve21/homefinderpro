﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using homefinderpro.Models;

namespace homefinderpro.AdminModels
{
    public class users : IUser
    {
        private readonly DBConnection _dbConnection;

        public ObjectId Id { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public byte[] ProfilePicture { get; set; }

        public users(DBConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public bool ApproveOrRejectPost(ObjectId postId, bool approve)
        {
            return _dbConnection.ApproveOrRejectPost(postId, approve);
        }
    }


}
