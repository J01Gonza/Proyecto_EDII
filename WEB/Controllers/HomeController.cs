using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using WEB.Models;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using DLL;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        const string SessionUser = "_User";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult SignIn() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> SignIn(IFormCollection collection)
        {
            try
            {
                var ingreso = GlobalVariables.webClient.GetAsync("User/AllUsers").Result;
                if (ingreso != null && ingreso.IsSuccessStatusCode)
                {
                    string p = await ingreso.Content.ReadAsStringAsync();
                    List<User> allusers = JsonConvert.DeserializeObject<List<User>>(p);
                    User sessionUser = allusers.Find(x => x.userName == collection["userName"]);
                    var k = sdesKeys(sessionUser.key);
                    string password = sdesEncode(sessionUser.password, k.key2, k.key1);
                    if (password == collection["password"])
                    {
                        HttpContext.Session.SetString(SessionUser, sessionUser.userName);
                        return RedirectToAction( "Index", "User");
                    }
                    else
                    {
                        ViewData["Error"] = "Contraseña incorrecta, viajero. Intenta nuevamente.";
                    }
                }
                else
                {
                    ViewData["Error"] = "Usuario no registrado, viajero. Dirígete a \"Crear cuenta\" para registrarte.";
                }
                return View();
            }
            catch (Exception e)
            {
                ViewData["Error"] = "Ingresa los datos correctamente, viajero.";
                return View();
            }
        }

        public IActionResult SignUp() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(IFormCollection collection)
        {
            try
            {
                var k = sdesKeys(collection["Name"].GetHashCode() % 256);
                string password = sdesEncode(collection["password"], k.key1, k.key2);
                var newUser = new User()
                {
                    userName = collection["userName"],
                    password = password,
                    name = collection["Name"],
                    lName = collection["LName"],
                    contacts = new List<Contact>(),
                    chats = new List<Chats>(),
                    key = collection["Name"].GetHashCode() % 256
                };
                string json = System.Text.Json.JsonSerializer.Serialize(newUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage ans = GlobalVariables.webClient.PostAsync("User/SignUp", content).Result;
                if (ans.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewData["Error"] = ans.ReasonPhrase;
                    return View();
                }
            }
            catch (Exception e)
            {
                string error = e.Message;
                ViewData["Error"] = "Ingresa los datos correctamente, viajero.";
                return View();
            }
        }

        public (string key1, string key2) sdesKeys(int key)
        {
            int[] P10 = { 3, 5, 2, 7, 4, 10, 1, 9, 8, 6 };
            int[] P8 = { 6, 3, 7, 4, 8, 5, 10, 9 };
            sdes SDES = new sdes();
            return SDES.generateKey(Convert.ToString(key, 2).PadLeft(10, '0'), P10, P8); ;
        }

        public string sdesEncode(string str, string key1, string key2)
        {
            List<byte> encode = new List<byte>();
            int[] P4 = { 2, 4, 3, 1 };
            int[] EP = { 4, 1, 2, 3, 2, 3, 4, 1 };
            int[] IP = { 2, 6, 3, 1, 4, 8, 5, 7 };
            int[] IP1 = { 4, 1, 3, 5, 7, 2, 8, 6 };
            sdes SDES = new sdes();
            byte[] bytes = Encoding.Unicode.GetBytes(str);
            foreach (var item in bytes)
            {
                string mainKey = Convert.ToString(item, 2).PadLeft(8, '0');
                encode.Add(SDES.Enconde(mainKey, key1, key2, P4, EP, IP, IP1));
            }
            return Encoding.Unicode.GetString(encode.ToArray());
        }
    }
}
