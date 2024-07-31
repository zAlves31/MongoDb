using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace minimalAPIMongo.Domains
{
    public class Order
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("date")]
        public DateOnly Date { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }



        [BsonElement("clientId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ClientId { get; set; }

        [BsonElement("client")]
        public Client? Client { get; set; }




        
            [BsonElement("productId")]
            [BsonRepresentation(BsonType.ObjectId)]
            public List<string>? ProductId { get; set; }

            [BsonElement("Product")]
            public List<Product>? Products { get; set; }
       


        [BsonElement("additionalAttributes")]
        public Dictionary<string, string> AdditionalAttributes { get; set; }

        public Order()
        {
            AdditionalAttributes = new Dictionary<string, string>();
        }
    }
}