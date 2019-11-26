using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back.Models;

namespace Back.Servicios
{
    public class UsuarioServicios
    {
        private readonly IMongoCollection<Usuario> _usuarios;

        public UsuarioServicios(IUsuariosDatabaseSettings configuracion)
        {
            var cliente = new MongoClient(configuracion.ConnectionString);
            var Base = cliente.GetDatabase(configuracion.DatabaseName);
            _usuarios = Base.GetCollection<Usuario>(configuracion.UsuarioCollectionName);
        }

        public List<Usuario> Get() =>
            _usuarios.Find(user => true).ToList();

        public Usuario Get(string id) =>
            _usuarios.Find<Usuario>(user => user.Id == id).FirstOrDefault();

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
            _usuarios.DeleteOne(us => us.Id == user.Id);
        }
        public void Remove(string id)
        {
            _usuarios.DeleteOne(user => user.Id == id);
        }
    }
}
