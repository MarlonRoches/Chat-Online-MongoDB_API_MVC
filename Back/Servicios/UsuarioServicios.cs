using Back.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Back.Servicios
{
    public class UsuarioServicios
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public UsuarioServicios(IUsuariosDatabaseSettings configuracion)
        {
            var cliente = new MongoClient(configuracion.ConnectionString);
            var Base = cliente.GetDatabase(configuracion.DatabaseName);
            _usuarios = Base.GetCollection<Usuario>(configuracion.UsuariosCollectionName);
        }

        public List<Usuario> Get() =>
            _usuarios.Find(user => true).ToList();

        public Usuario Get(string us) =>
            _usuarios.Find<Usuario>(user => user.NombreUsuario == us).FirstOrDefault();

        public Usuario Create(Usuario user)
        {
            _usuarios.InsertOne(user);
            return user;
        }
        public void Update(string id, Usuario entrada)
        {
            _usuarios.ReplaceOne(user => user.Id == id, entrada);

        }
        public void Remove(Usuario user)
        {
            _usuarios.DeleteOne(us => us.NombreUsuario == user.Id);
        }
        public void Remove(string id)
        {
            _usuarios.DeleteOne(user => user.Id == id);
        }
    }
}
