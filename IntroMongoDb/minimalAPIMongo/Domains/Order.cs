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
        public DateTime Date { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; }

        [BsonElement("products")]
        public List<ProductReference> Products { get; set; } = new List<ProductReference>();

        [BsonElement("client")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ClientId { get; set; }

        public class ProductReference
        {
            [BsonElement("productId")]
            [BsonRepresentation(BsonType.ObjectId)]
            public string? ProductId { get; set; }

            [BsonElement("quantity")]
            public int Quantity { get; set; }
        }


        [BsonElement("additionalAttributes")]
        public Dictionary<string, string> AdditionalAttributes { get; set; }

        public Order()
        {
            AdditionalAttributes = new Dictionary<string, string>();
        }
    }
}