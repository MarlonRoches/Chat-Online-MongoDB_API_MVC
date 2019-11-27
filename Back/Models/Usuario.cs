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
<<<<<<< HEAD
        public string NombreUsuario { get; set; }
<<<<<<< HEAD
=======
        public string CorreoElectronico { get; set; }
=======
        public string User { get; set; }
        public string eMail { get; set; }
        public int LlaveSDES { get; set; }
>>>>>>> ebb764c2036dfd10deafdaab00c162867c906ed5
        public List<string> Contactos { get; set; }
>>>>>>> 92490af11df318f49c2714361f1e421e913a4f23

    }
}
