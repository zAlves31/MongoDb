using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace minimalAPIMongo.Domains
{
    public class User
    {
        //define que esta prop e Id do objeto
        [BsonId]
        //define o nome do campo no MongoDb como "_id" e o tipo como ObjectId
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("password")]
        public string? Password { get; set; }

        //adiciona um dicionario para atributos adicionais
        public Dictionary<string, string> AddtionalAttributes { get; set; }

        /// <summary>
        /// Ao ser instanciado um obj da classe Product, o atributo AddtionalAttributes ja vira com um novo dicionario e portanto habilitado para adicionar mais atributos
        /// </summary>
        public User()
        {
            AddtionalAttributes = new Dictionary<string, string>();
        }
    }
}
