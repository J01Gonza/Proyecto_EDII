using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Contact
    {
        public string UserContact { get; set; }
        public bool Sent { get; set; }
        public bool Received { get; set; }
    }
}
