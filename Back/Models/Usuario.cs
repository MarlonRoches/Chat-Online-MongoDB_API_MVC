using MongoDB.Bson.Serialization.Attributes;

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

    }
}
