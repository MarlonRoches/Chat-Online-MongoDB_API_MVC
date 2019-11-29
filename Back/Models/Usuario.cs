using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Back.Models
{
    public class Usuario
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Usuario")]

        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Password { get; set; }
        public string User { get; set; }
        public string eMail { get; set; }
        public int LlaveSDES { get; set; }
        public List<string> Contactos = new List<string>();

    }
}
