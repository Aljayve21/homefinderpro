﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.AdminModels
{
    public class AdminApproval
    {
        public ObjectId Id { get; set; }
        public ObjectId LandlordId { get; set; }
        public ObjectId LandlordPostId { get; set; }
        public string ApprovalStatus { get; set; }
        
    }
}
