using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WEB.Models
{
    public class Chats
    {
        public int id { get; set; }
        public List<Messages> messages { get; set; }
        public bool group { get; set; }
        public List<string> members { get; set; }
        public List<int> keys { get; set; }
    }
}
