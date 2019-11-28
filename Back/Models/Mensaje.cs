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
        public Dictionary<DateTime, Extesiones> EmisorMen = new Dictionary<DateTime, Extesiones>();
        public Dictionary<DateTime, Extesiones> ReceptorMen = new Dictionary<DateTime, Extesiones>();  

        public string                       Emisor              { get; set; }
        public string                       Receptor            { get; set; }
        

    }
}
