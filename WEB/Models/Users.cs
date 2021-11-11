using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Models
{
    public class Usuario
    {
        [BsonId]
        public string user { get; set; }
        public string password { get; set; }
        public List<Chats> Chats { get; set; }
        public List<string> contactos { get; set; }
        public int clave { get; set; }
    }
}
