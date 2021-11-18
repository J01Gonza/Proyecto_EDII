using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WEB
{
    public class GlobalVariables
    {
        public static HttpClient webClient = new HttpClient();
        static GlobalVariables()
        {
            webClient.BaseAddress = new Uri("http://localhost:47391/api/");
        }
    }
}
