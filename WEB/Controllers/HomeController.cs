using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using WEB.Models;
using System.Collections.Generic;
using System.Net.Http;

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
                //descifrar contraseña antes de hacer esto
                User Ingreso = DB.AllUsers().Find(x => x.User.Equals(collection["User"]) && x.Password.Equals(collection["Password"]));
                if (Ingreso != null)
                {
                    HttpContext.Session.SetString(SessionUser, Ingreso.User);
                    return RedirectToAction("Index", "User", new { user = collection["User"]});
                }
                else
                {
                    ViewData["Error"] = "Usuario no registrado, Dirigase a la pestaña de Sign Up";
                }
                    return View();
            }
            catch (Exception)
            {
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
                    User = collection["User"],
                    //CIFRAR CONTRASEÑA
                    Password = collection["Password"],
                    Name = collection["Name"],
                    LName = collection["LName"],
                    Contacts = new List<Contact>(),
                    Chats = new List<Chats>(),
                    Key = collection["Name"].GetHashCode()
                };
                try
                {
                    HttpResponseMessage ans = GlobalVariables.webClient.PostAsJsonAsync("User", newUser).Result; 
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
