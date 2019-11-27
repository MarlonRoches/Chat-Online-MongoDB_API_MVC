using Back.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Back.Servicios
{
    public class ChatsServicios
    {
        private readonly IMongoCollection<Chat> _Chats;

        public ChatsServicios(IChatsDatabaseSettings configuracion)
        {
            var cliente = new MongoClient(configuracion.ConnectionString);
            var Base = cliente.GetDatabase(configuracion.DatabaseName);
            _Chats = Base.GetCollection<Chat>(configuracion.ChatCollectionName);
        }

        public List<Chat> Get() =>
            _Chats.Find(_chat => true).ToList();

        public Chat Get(string id) =>
            _Chats.Find<Chat>(_chat => _chat.Id == id).FirstOrDefault();

        public Chat Create(Chat _chat)
        {
            _Chats.InsertOne(_chat);
            return _chat;
        }
        public void Update(string id, Chat entrada)
        {
            _Chats.ReplaceOne(_chat => _chat.Id == id, entrada);

        }
        public void Remove(Chat _chats)
        {
            _Chats.DeleteOne(_chat => _chat.Id == _chats.Id);
        }
        public void Remove(string id)
        {
            _Chats.DeleteOne(_cha => _cha.Id == id);
        }
    }

}
