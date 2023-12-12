using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homefinderpro.LandlordModels
{
    public class LandlordPost
    {
        public ObjectId Id { get; set; }
        public string ApartmentName { get; set; }
        public decimal Price { get; set; }
        public string Descriptions { get; set; }
        public string Location { get; set; }
        public List<byte[]> Pictures { get; set; }
        public byte[] GovernmentDocument { get; set; }
        public byte[] ValidId { get; set; }
        public bool IsApproved { get; set; }
    }
}
