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
                Usuario Ingreso = DB.AllUsers().Find(x => x.User.Equals(collection["User"]) && x.Password.Equals(collection["Password"]));
                if (Ingreso != null)
                {
                    HttpContext.Session.SetString(SessionUser, Ingreso.User);
                    return RedirectToAction(nameof(Home));
                }
                else
                {
                    ViewData["Error"] = "Usuario no registrado, Dirigase a la pestaña de Sign Up";
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
                    Contacts = new List<Contacto>(),
                    Chats = new List<Chats>(),
                    Key = collection["Name"].GetHashCode()
                };
                try
                {
                    DB.NewUser(newUser);
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

        public IActionResult Home() {
            string usuario = HttpContext.Session.GetString(SessionUser);
            List<Chats> chats = DB.AllUsers().Find(x => x.User.Equals(usuario)).Chats;
            return View(chats); 
        }
        
        public IActionResult AddFriend()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddFriend(IFormCollection collection)
        {
            try
            {
                Usuario Ingreso = DB.AllUsers().Find(x => x.User.Equals(collection["User"]));
                Usuario Sesion = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString(SessionUser)));
                if (Ingreso != null) {
                    Contacto newContact = new Contacto()
                    {
                        UserContact = collection["User"],
                        Sent = true,
                        Received = false
                    };
                    if (Sesion.Contacts.Find(x => x.UserContact.Equals(newContact.UserContact)) == null)
                    {
                        Sesion.Contacts.Add(newContact);
                        Ingreso.Contacts.Add(new Contacto() { UserContact = Sesion.User, Sent = false, Received = true });
                        DB.UpdateUser(Sesion);
                        DB.UpdateUser(Ingreso);
                        ViewData["Success"] = "Solicitud de amistad enviada a " + collection["User"];
                    }
                    else{
                        ViewData["Error"] = "Contacto ya existente";
                    }
                }
                else
                {
                    ViewData["Error"] = "Usuario no encontrado en la base de datos";
                }
            }
            catch
            {
                ViewData["Error"] = "Información ingresada no válida, intente de nuevo";
            }
            return View();
        }

        public IActionResult Contacts() {
            string usuario = HttpContext.Session.GetString(SessionUser);
            List<Contacto> contacto= DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == true && x.Received == true);
            return View(contacto); 
        }

        public IActionResult SentRequests()
        {
            string usuario = HttpContext.Session.GetString(SessionUser);
            List<Contacto> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == true && x.Received == false);
            return View(contacto);
        }

        public IActionResult Requests()
        {
            string usuario = HttpContext.Session.GetString(SessionUser);
            List<Contacto> contacto = DB.AllUsers().Find(x => x.User.Equals(usuario)).Contacts.FindAll(x => x.Sent == false && x.Received == true);
            return View(contacto);
        }
        public IActionResult AcceptRequests(string id)
        {
            Usuario upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString(SessionUser)));
            Usuario upUsuario2 = DB.AllUsers().Find(x => x.User.Equals(id));
            int x = upUsuario.Contacts.FindIndex(x => x.UserContact.Equals(id));
            int x2 = upUsuario2.Contacts.FindIndex(x => x.UserContact.Equals(HttpContext.Session.GetString(SessionUser)));
            upUsuario.Contacts[x].Sent = true;
            upUsuario2.Contacts[x2].Received = true;
            DB.UpdateUser(upUsuario);
            DB.UpdateUser(upUsuario2);
            return RedirectToAction(nameof(Contacts));
        }
        public IActionResult DeleteRequests(string id, bool sent)
        {
            Usuario upUsuario = DB.AllUsers().Find(x => x.User.Equals(HttpContext.Session.GetString(SessionUser)));
            Usuario upUsuario2 = DB.AllUsers().Find(x => x.User.Equals(id));
            int x = upUsuario.Contacts.FindIndex(x => x.UserContact.Equals(id));
            int x2 = upUsuario2.Contacts.FindIndex(x => x.UserContact.Equals(HttpContext.Session.GetString(SessionUser)));
            upUsuario.Contacts.RemoveAt(x);
            upUsuario2.Contacts.RemoveAt(x2);
            DB.UpdateUser(upUsuario);
            DB.UpdateUser(upUsuario2);
            ViewData["Success"] = "Solicitud de amistad eliminada correctamente";
            if (sent)
            {
                return RedirectToAction(nameof(SentRequests));
            }
            else
            {
                return RedirectToAction(nameof(Request));
            }
        }
    }
}
