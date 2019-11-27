using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Back.Models
{
    public class Chat
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Chat")]
        List<Mensaje> Emisor { get; set; }
        List<Mensaje> Receptor { get; set; }
    }
}
