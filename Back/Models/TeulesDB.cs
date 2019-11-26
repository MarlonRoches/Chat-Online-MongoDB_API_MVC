using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Back.Models
{
    //public class TeulesDB
    //{
        public class UsuariosDatabaseSettings : IUsuariosDatabaseSettings
        {
            public string UsuarioCollectionName { get; set; }
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        public interface IUsuariosDatabaseSettings
        {
            string UsuarioCollectionName { get; set; }
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }
        public class MensajesDatabaseSettings : IMensajesDatabaseSettings
        {
            public string MensajeCollectionName { get; set; }
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        public interface IMensajesDatabaseSettings
        {
            string MensajeCollectionName { get; set; }
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }
        public class ChatsDatabaseSettings : IChatsDatabaseSettings
        {
            public string ChatCollectionName { get; set; }
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        public interface IChatsDatabaseSettings
        {
            string ChatCollectionName { get; set; }
            string ConnectionString { get; set; }
            string DatabaseName { get; set; }
        }
    //}
}
