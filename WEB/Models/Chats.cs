using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Models
{
    public class Chats
    {
        public List<Messages> messages { get; set; }
        public bool group { get; set; }
        public List<string> members { get; set; }
        public List<string> keys { get; set; }
    }
}
