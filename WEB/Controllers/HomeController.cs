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
        public IActionResult SignIn(IFormCollection collection)
        {
            try
            {
                string uri = "User/UserbyUN/" + collection["userName"];
                var ingreso = GlobalVariables.webClient.GetAsync(uri).Result;
                if (ingreso != null)
                {
                    string json = ingreso.Content.ReadAsStringAsync().Result; 
                    User us = System.Text.Json.JsonSerializer.Deserialize<User>(json);
                    //DESCIFRAR CONTRASEÑA
                    if (us.password == collection["password"])
                    {
                        //ingresa sesión
                    }
                    else
                    {
                        ViewData["Error"] = "Contraseña incorrecta. Intenta de nuevo, viajero";
                    }
                }
                else
                {
                    ViewData["Error"] = "Usuario no registrado, Dirigase a la pestaña de Sign Up";
                }
                return View();
            }
            catch (Exception e)
            {
                string x = e.Message;
                ViewData["Error"] = "Ingrese correctamente los datos";
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
                var newUser = new User()
                {
                    userName = collection["userName"],
                    //CIFRAR CONTRASEÑA
                    password = collection["Password"],
                    name = collection["Name"],
                    lName = collection["LName"],
                    contacts = new List<Contact>(),
                    chats = new List<Chats>(),
                    key = collection["Name"].GetHashCode() % 256
                };
                try
                {
                    string json = System.Text.Json.JsonSerializer.Serialize(newUser);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage ans = GlobalVariables.webClient.PostAsync("User/SignUp", content).Result; 
                    ViewData["Success"] = "Usuario registrado exitosamente";
                }
                catch (Exception e)
                {
                    ViewData["Error"] = e.Message;
                }
                return View();    
            }
            catch (Exception e)
            {
                string error = e.Message;
                ViewData["Error"] = "Ingrese correctamente los datos, por favor";
                return View();
            }
        }
        
    }
}
