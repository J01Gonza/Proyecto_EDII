using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string id { get; set; }
        [BsonElement("Usuario")]
        [Display(Name = "Usuario")]
        [Required]
        public string userName { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        public string name { get; set; }
        [Display(Name = "Apellido")]
        [Required]
        public string lName { get; set; }
        [Display(Name = "Contraseña")]
        [Required]
        public string password { get; set; }
        public List<Chats> chats { get; set; }
        public List<Contact> contacts { get; set; }
        public int key { get; set; }
        public string contactSelected { get; set; }
    }
}
