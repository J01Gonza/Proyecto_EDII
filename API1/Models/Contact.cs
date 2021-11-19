using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.Models
{
    public class Contact
    {
        public string userContact { get; set; }
        public bool sent { get; set; }
        public bool received { get; set; }
    }
}
