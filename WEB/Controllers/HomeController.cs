using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using WEB.Models;
using WEB.Conexiones;
using System.Collections.Generic;

namespace WEB.Controllers
{
    public class HomeController : Controller
    {
        private IUsersCollection DB = new UsersCollection();
        private readonly ILogger<HomeController> _logger;
        const string SessionUser = "_User";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            ViewBag.Session = HttpContext.Session.GetString(SessionUser);
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
                Usuario Ingreso = DB.AllUsers().Find(x => x.User.Equals(collection["user"]) && x.Password.Equals(""));
                if (Ingreso != null)
                {
                    HttpContext.Session.SetString(SessionUser, Ingreso.User);
                    return RedirectToAction(nameof(Home));
                }
                else
                {

                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult SignUp() { return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(IFormCollection collection)
        {
            try
            {

                var newUser = new Usuario()
                {
                    User = collection["User"],
                    //CIFRAR CONTRASEÑA
                    Password = collection["Password"],
                    Name = collection["Name"],
                    LName = collection["LName"],
                    Contacts = new List<string>(),
                    Chats = new List<string>(),
                    Key = collection["Name"].GetHashCode()
                };
                    DB.NewUser(newUser);
                    ViewData["Success"] = "Usuario creado correctamente :)";
                    return View();
            }
            catch (Exception e)
            {
                string error = e.Message;
                ViewData["Error"] = "Ingrese correctamente los datos, por favor";
                return View();
            }
        }

        public IActionResult Home() { return View(); }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Home(IFormCollection collection) { 
            return View(); 
        }
    }
}
