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
        public string NombreUsuario { get; set; }
        public string CorreoElectronico { get; set; }
        public int LlaveSDES { get; set; }
        public List<string> Contactos { get; set; }

    }
}
