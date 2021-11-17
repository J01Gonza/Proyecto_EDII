using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WEB.Models
{
    public class Mensajes
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        public IFormFile File { get; set; }
    }
}
