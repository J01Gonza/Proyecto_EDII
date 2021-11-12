using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Models
{
    public class Contacto 
    {
        [Display(Name = "Usuario")]
        [Required]
        public string UserContact { get; set; }
        public bool Sent { get; set; }
        public bool Received { get; set; }
    }
}
