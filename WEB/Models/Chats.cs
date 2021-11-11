using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Models
{
    public class Chats
    {
        public List<Mensajes> mensajes { get; set; }
        public bool Grupal { get; set; }
        public List<string> usuarios { get; set; }
        public List<char> claves { get; set; }
    }
}
