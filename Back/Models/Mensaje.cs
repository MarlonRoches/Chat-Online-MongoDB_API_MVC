using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Back.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Mensaje")]
        public int ID { get; set; }
        public string Texto { get; set; }
        public string Archivo { get; set; }
        public string Emisor { get; set; }
        public string Receptor { get; set; }
        public DateTime Fecha { get; set; }

    }
}
