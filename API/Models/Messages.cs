using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Messages
    {
        public string Content { get; set; }
        public string Sender { get; set; }
        IFormFile File { get; set; }
    }
}
