using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Back.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string                       Id                  { get; set; }
        [BsonElement("Mensaje")]
        public string                       IDEmisorReceptor    { get; set; }
        public Dictionary<DateTime, string> Mensjaes            { get; set; }
        public Dictionary<DateTime, string> Archivos            { get; set;}
        public string                       Emisor              { get; set; }
        public string                       Receptor            { get; set; }
        

    }
}
