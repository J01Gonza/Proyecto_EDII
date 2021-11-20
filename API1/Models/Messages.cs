﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API1.Models
{
    public class Messages
    {
        public int id { get; set; }
        public string content { get; set; }
        public string sender { get; set; }
        public File file { get; set; }
    }
}
