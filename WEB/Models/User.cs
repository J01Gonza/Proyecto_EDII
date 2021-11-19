using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Models
{
    public class User
    {
        [BsonId]
        public ObjectId id { get; set; }
        
        public string userName { get; set; }
        
        public string name { get; set; }
       
        public string lName { get; set; }
       
        public string password { get; set; }
        public List<Chats> chats { get; set; }
        public List<Contact> contacts { get; set; }
        public int key { get; set; }
    }
}
