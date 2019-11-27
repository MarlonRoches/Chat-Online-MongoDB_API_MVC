using Back.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Back.Servicios
{
    public class MensajesServicios
    {
        private readonly IMongoCollection<Mensaje> _Mensajes;

        public MensajesServicios(IMensajesDatabaseSettings configuracion)
        {
            var cliente = new MongoClient(configuracion.ConnectionString);
            var Base = cliente.GetDatabase(configuracion.DatabaseName);
            _Mensajes = Base.GetCollection<Mensaje>(configuracion.MensajeCollectionName);
        }

        public List<Mensaje> Get() =>
            _Mensajes.Find(_men => true).ToList();

        public Mensaje Get(string id) =>
            _Mensajes.Find<Mensaje>(_men => _men.IDEmisorReceptor == id).FirstOrDefault();

        public Mensaje Create(Mensaje _men)
        {
            _Mensajes.InsertOne(_men);
            return _men;
        }
        public void Update(string id, Mensaje entrada)
        {
            _Mensajes.ReplaceOne(men => men.Id == id, entrada);

        }
        public void Remove(Mensaje _men)
        {
            _Mensajes.DeleteOne(Men => Men.Id == _men.Id);
        }
        public void Remove(string id)
        {
            _Mensajes.DeleteOne(Men => Men.Id == id);
        }
    }

}
