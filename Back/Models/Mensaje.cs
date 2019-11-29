using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Back.Models
{
    public class Mensaje
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string                           Id                  { get; set; }
        [BsonElement("Mensaje")]
        public string                           IDEmisorReceptor    { get; set; }
        public Dictionary<string, Extesiones>   EmisorMen           = new Dictionary<string, Extesiones>();
        public Dictionary<string, Extesiones>   ReceptorMen         = new Dictionary<string, Extesiones>();  

        public string                           Emisor              { get; set; }
        public string                           Receptor            { get; set; }
        public Dictionary<string,bool>          MensajesOrdenados   = new Dictionary<string, bool>();

        

    }
}
