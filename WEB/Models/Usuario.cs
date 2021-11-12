﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WEB.Models
{
    public class Usuario
    {
        [BsonId]
        public ObjectId ID { get; set; }
        [Display (Name = "Usuario")]
        [Required]
        public string User { get; set; }
        [Display(Name = "Nombre")]
        [Required]
        public string Name { get; set; }
        [Display(Name = "Apellido")]
        [Required]
        public string LName { get; set; }
        [Display(Name = "Contraseña")]
        [Required]
        public string Password { get; set; }
        public List<Chats> Chats { get; set; }
        public List<Contacto> Contacts { get; set; }
        public int Key { get; set; }
    }
}