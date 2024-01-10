using MongoDB.Bson;
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
        public string Username { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string Category { get; set; }

        public string Description { get; set; }
        public string Location { get; set; }

        public decimal Price { get; set; }

        public List<byte[]> Photos { get; set; }

        public byte[] ValidIdPicture { get; set; }

        public byte[] GovernmentDocument { get; set; }

        public string Status { get; set; }

        public DateTime SubmissionDate { get; set; }

        public List<ObjectId> PhotoIds { get; set; }
    }
}
