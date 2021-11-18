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
        GlobalVariables()
        {
            webClient.BaseAddress = new Uri("http://localhost:12455/api");
            webClient.DefaultRequestHeaders.Clear();
            webClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        }
    }
}
