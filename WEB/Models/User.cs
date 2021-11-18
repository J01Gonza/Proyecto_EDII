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
        public ObjectId ID { get; set; }
        
        public string userName { get; set; }
        
        public string Name { get; set; }
       
        public string LName { get; set; }
       
        public string Password { get; set; }
        public List<Chats> Chats { get; set; }
        public List<Contact> Contacts { get; set; }
        public int Key { get; set; }
    }
}
